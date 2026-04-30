import importlib.util
import pathlib
import unittest
from unittest import mock

import cv2
import numpy as np


SCRIPT_PATH = pathlib.Path(__file__).resolve().parents[1] / "scripts" / "sprite_matcher.py"
SPEC = importlib.util.spec_from_file_location("sprite_matcher", SCRIPT_PATH)
sprite_matcher = importlib.util.module_from_spec(SPEC)
assert SPEC.loader is not None
SPEC.loader.exec_module(sprite_matcher)


class SpriteMatcherTests(unittest.TestCase):
    def test_soft_match_accepts_non_exact_but_high_quality_region(self):
        expected = np.full((40, 40, 3), 100, dtype=np.uint8)
        region = expected.copy()
        region[:12, :12] = 160
        valid_mask = np.ones((40, 40), dtype=bool)

        result = sprite_matcher._analyze_region_match(region, expected, valid_mask, max_diff=40)

        self.assertTrue(result["accepted"])
        self.assertFalse(result["perfect"])
        self.assertGreaterEqual(result["match_ratio"], sprite_matcher.SOFT_MATCH_RATIO_MIN)

    def test_small_region_still_requires_stricter_ratio(self):
        expected = np.full((16, 16, 3), 100, dtype=np.uint8)
        region = expected.copy()
        region[:6, :6] = 160
        valid_mask = np.ones((16, 16), dtype=bool)

        result = sprite_matcher._analyze_region_match(region, expected, valid_mask, max_diff=40)

        self.assertFalse(result["accepted"])

    def test_fuzzy_accepts_localized_mismatch_block(self):
        expected = np.full((40, 40, 3), 100, dtype=np.uint8)
        region = expected.copy()
        region[10:24, 12:26] = 180
        valid_mask = np.ones((40, 40), dtype=bool)

        result = sprite_matcher._analyze_region_match(
            region, expected, valid_mask, max_diff=24,
            min_match_ratio=sprite_matcher.FUZZY_MATCH_RATIO_MIN,
            small_match_ratio=sprite_matcher.FUZZY_SMALL_MATCH_RATIO_MIN,
            allow_local_diff_rescue=True)

        self.assertTrue(result["accepted"])
        self.assertTrue(result["local_diff_rescued"])
        self.assertEqual(196, result["local_diff_pixels"])

    def test_fuzzy_rejects_scattered_mismatch_pixels(self):
        expected = np.full((40, 40, 3), 100, dtype=np.uint8)
        region = expected.copy()
        ys, xs = np.indices((40, 40))
        scatter = ((xs * 7 + ys * 11) % 9) == 0
        region[scatter] = 180
        valid_mask = np.ones((40, 40), dtype=bool)

        result = sprite_matcher._analyze_region_match(
            region, expected, valid_mask, max_diff=24,
            min_match_ratio=sprite_matcher.FUZZY_MATCH_RATIO_MIN,
            small_match_ratio=sprite_matcher.FUZZY_SMALL_MATCH_RATIO_MIN,
            allow_local_diff_rescue=True)

        self.assertFalse(result["accepted"])

    def test_fuzzy_accepts_large_focused_inner_difference(self):
        expected = np.full((64, 64, 3), 100, dtype=np.uint8)
        region = expected.copy()
        region[14:50, 14:50] = 180
        valid_mask = np.ones((64, 64), dtype=bool)

        result = sprite_matcher._analyze_region_match(
            region, expected, valid_mask, max_diff=24,
            min_match_ratio=sprite_matcher.FUZZY_MATCH_RATIO_MIN,
            small_match_ratio=sprite_matcher.FUZZY_SMALL_MATCH_RATIO_MIN,
            allow_local_diff_rescue=True)

        self.assertTrue(result["accepted"])
        self.assertTrue(result["focused_diff_rescued"])
        self.assertEqual(1296, result["focused_diff_pixels"])

    def test_fuzzy_majority_rescue_accepts_high_corr_majority_match(self):
        verify = {
            "accepted": False,
            "match_ratio": 0.6734,
            "median_diff": 1.0,
            "valid_pixels": 7585,
            "majority_rescued": False,
        }

        accepted = sprite_matcher._should_accept_fuzzy_majority(0.9317, verify)

        self.assertTrue(accepted)
        self.assertTrue(verify["accepted"])
        self.assertTrue(verify["majority_rescued"])

    def test_deduplicate_keeps_overlapping_different_sprites(self):
        matches = [
            {
                "sprite_idx": 1,
                "is_white": False,
                "rect": {"x": 100, "y": 100, "width": 120, "height": 120},
                "median_diff": 1.0,
                "match_ratio": 0.95,
                "candidate_score": 0.99,
            },
            {
                "sprite_idx": 2,
                "is_white": False,
                "rect": {"x": 100, "y": 100, "width": 120, "height": 120},
                "median_diff": 2.0,
                "match_ratio": 0.92,
                "candidate_score": 0.97,
            },
        ]

        deduped = sprite_matcher.deduplicate_matches(matches)

        self.assertEqual(2, len(deduped))

    def test_deduplicate_merges_same_sprite_same_rect(self):
        matches = [
            {
                "sprite_idx": 1,
                "is_white": False,
                "rect": {"x": 100, "y": 100, "width": 120, "height": 120},
                "median_diff": 1.0,
                "match_ratio": 0.95,
                "candidate_score": 0.99,
            },
            {
                "sprite_idx": 1,
                "is_white": False,
                "rect": {"x": 101, "y": 100, "width": 121, "height": 119},
                "median_diff": 2.0,
                "match_ratio": 0.92,
                "candidate_score": 0.97,
            },
        ]

        deduped = sprite_matcher.deduplicate_matches(matches)

        self.assertEqual(1, len(deduped))

    def test_deduplicate_merges_same_sprite_across_original_and_white_paths(self):
        matches = [
            {
                "sprite_idx": 3,
                "is_white": False,
                "rect": {"x": 200, "y": 120, "width": 44, "height": 44},
                "median_diff": 1.0,
                "match_ratio": 0.98,
                "candidate_score": 0.99,
            },
            {
                "sprite_idx": 3,
                "white_idx": 0,
                "is_white": True,
                "rect": {"x": 200, "y": 120, "width": 44, "height": 44},
                "median_diff": 2.0,
                "match_ratio": 0.95,
                "candidate_score": 0.97,
            },
        ]

        deduped = sprite_matcher.deduplicate_matches(matches)

        self.assertEqual(1, len(deduped))
        self.assertFalse(deduped[0]["is_white"])

    def test_load_sprites_keeps_white_sprites_in_original_match_pool(self):
        img = np.zeros((16, 16, 4), dtype=np.uint8)
        img[4:12, 7:9] = [255, 255, 255, 255]
        img[7:9, 4:12] = [255, 255, 255, 255]

        with mock.patch.object(
            sprite_matcher.os,
            "walk",
            return_value=[("C:/fake_sprites", [], ["Icon_Close01 1.png"])],
        ), mock.patch.object(sprite_matcher.cv2, "imread", return_value=img):
            sprites, sliced_indices, white_sprites = sprite_matcher.load_sprites(
                ["C:/fake_sprites"],
                {},
                {"Icon_Close01 1.png": {"color": [255, 255, 255], "alpha_coverage": 0.25}},
            )

        self.assertEqual(1, len(sprites))
        self.assertEqual([], sliced_indices)
        self.assertEqual(1, len(white_sprites))
        self.assertEqual("Icon_Close01 1.png", sprites[0]["rel_path"])
        self.assertEqual(0, sprites[0]["base_sprite_idx"])
        self.assertIs(sprites[0], white_sprites[0])

    def test_load_sprites_auto_detects_binary_black_white_as_white_source(self):
        img = np.zeros((16, 16, 4), dtype=np.uint8)
        img[3:13, 3:13] = [255, 255, 255, 255]
        img[6:10, 6:10] = [0, 0, 0, 255]

        with mock.patch.object(
            sprite_matcher.os,
            "walk",
            return_value=[("C:/fake_sprites", [], ["BinaryBW.png"])],
        ), mock.patch.object(sprite_matcher.cv2, "imread", return_value=img):
            sprites, sliced_indices, white_sprites = sprite_matcher.load_sprites(
                ["C:/fake_sprites"],
                {},
                {},
            )

        self.assertEqual(1, len(sprites))
        self.assertEqual([], sliced_indices)
        self.assertEqual(1, len(white_sprites))
        self.assertTrue(sprites[0]["is_white_source"])

    def test_load_sprites_auto_detects_grayscale_sprite_as_white_source(self):
        img = np.zeros((16, 16, 4), dtype=np.uint8)
        img[3:13, 3:13] = [220, 220, 220, 255]
        img[5:11, 5:11] = [64, 64, 64, 255]
        img[7:9, 7:9] = [0, 0, 0, 255]

        with mock.patch.object(
            sprite_matcher.os,
            "walk",
            return_value=[("C:/fake_sprites", [], ["GrayScale.png"])],
        ), mock.patch.object(sprite_matcher.cv2, "imread", return_value=img):
            sprites, sliced_indices, white_sprites = sprite_matcher.load_sprites(
                ["C:/fake_sprites"],
                {},
                {},
            )

        self.assertEqual(1, len(sprites))
        self.assertEqual([], sliced_indices)
        self.assertEqual(1, len(white_sprites))
        self.assertTrue(sprites[0]["is_white_source"])

    def test_load_sprites_does_not_auto_detect_non_grayscale_sprite_as_white_source(self):
        img = np.zeros((16, 16, 4), dtype=np.uint8)
        img[3:13, 3:13] = [255, 255, 255, 255]
        img[6:10, 6:10] = [0, 0, 255, 255]

        with mock.patch.object(
            sprite_matcher.os,
            "walk",
            return_value=[("C:/fake_sprites", [], ["Colored.png"])],
        ), mock.patch.object(sprite_matcher.cv2, "imread", return_value=img):
            sprites, sliced_indices, white_sprites = sprite_matcher.load_sprites(
                ["C:/fake_sprites"],
                {},
                {},
            )

        self.assertEqual(1, len(sprites))
        self.assertEqual(0, len(white_sprites))
        self.assertFalse(sprites[0]["is_white_source"])

    def test_parse_match_scales_arg_normalizes_and_deduplicates(self):
        parsed = sprite_matcher.parse_match_scales_arg("1.1, 1.0, 1.1, 0, bad, 0.9")

        self.assertEqual([1.1, 1.0, 0.9], parsed)
        self.assertEqual([1.1, 1.0, 0.9], sprite_matcher.normalize_match_scales("1.1, 1.0, 1.1, 0, bad, 0.9"))
        self.assertEqual(
            sprite_matcher.DEFAULT_FUZZY_SCALES,
            sprite_matcher.normalize_match_scales(None),
        )

    def test_match_normal_skips_border_sprites(self):
        img = np.zeros((24, 24, 4), dtype=np.uint8)
        img[:, :, :3] = [40, 40, 220]
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/Underlay.png",
            "rel_path": "Underlay.png",
            "img": img,
            "w": 24,
            "h": 24,
            "ar": 1.0,
            "border": {"left": 4, "right": 4, "top": 4, "bottom": 4},
            "base_sprite_idx": 0,
            "is_white_source": False,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.zeros((48, 48, 3), dtype=np.uint8)
        screenshot[10:34, 12:36] = img[:, :, :3]

        overlay = np.zeros((48, 48, 4), dtype=np.uint8)
        existing = []

        matches = sprite_matcher._match_normal(
            [sprite], [], screenshot, overlay, existing)

        self.assertEqual(0, len(matches))

    def test_original_match_requires_exact_pixels(self):
        yy, xx = np.indices((12, 12))
        img = np.zeros((12, 12, 4), dtype=np.uint8)
        img[:, :, 0] = (xx * 17 + yy * 13) % 256
        img[:, :, 1] = (xx * 9 + yy * 21 + 40) % 256
        img[:, :, 2] = (xx * 5 + yy * 7 + 90) % 256
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/Exact.png",
            "rel_path": "Exact.png",
            "img": img,
            "w": 12,
            "h": 12,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "is_white_source": False,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.zeros((32, 32, 3), dtype=np.uint8)
        screenshot[7:19, 9:21] = img[:, :, :3]

        matches = sprite_matcher._match_normal(
            [sprite], [], screenshot, None, [])

        self.assertEqual(1, len(matches))
        self.assertEqual({"x": 9, "y": 7, "width": 12, "height": 12}, matches[0]["rect"])
        self.assertEqual(1.0, matches[0]["match_ratio"])
        self.assertEqual(0.0, matches[0]["median_diff"])

    def test_original_match_accepts_near_perfect_with_small_diff(self):
        yy, xx = np.indices((12, 12))
        img = np.zeros((12, 12, 4), dtype=np.uint8)
        img[:, :, 0] = (xx * 17 + yy * 13) % 256
        img[:, :, 1] = (xx * 9 + yy * 21 + 40) % 256
        img[:, :, 2] = (xx * 5 + yy * 7 + 90) % 256
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/Exact.png",
            "rel_path": "Exact.png",
            "img": img,
            "w": 12,
            "h": 12,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "is_white_source": False,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.zeros((32, 32, 3), dtype=np.uint8)
        screenshot[7:19, 9:21] = img[:, :, :3]
        screenshot[10, 12] = [31, 140, 220]

        matches = sprite_matcher._match_normal(
            [sprite], [], screenshot, None, [])

        self.assertEqual(1, len(matches))
        self.assertGreater(matches[0]["match_ratio"], 0.9)

    def test_fuzzy_refines_scale_beyond_bucket(self):
        yy, xx = np.indices((128, 128))
        img = np.zeros((128, 128, 4), dtype=np.uint8)
        img[:, :, 0] = (xx * 11 + yy * 5 + 20) % 256
        img[:, :, 1] = (xx * 7 + yy * 13 + 60) % 256
        img[:, :, 2] = (xx * 3 + yy * 17 + 100) % 256
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/ScaledIcon.png",
            "rel_path": "ScaledIcon.png",
            "img": img,
            "w": 128,
            "h": 128,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "is_white_source": False,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.zeros((220, 220, 3), dtype=np.uint8)
        y0, x0 = 24, 36
        scaled = sprite_matcher.resize_sprite(sprite, 143, 143)
        screenshot[y0:y0 + 143, x0:x0 + 143] = scaled[:, :, :3]

        matches = sprite_matcher._match_normal(
            [sprite], [], screenshot, None, [],
            fuzzy_scales=[1.0, 1.1])

        self.assertEqual(1, len(matches))
        self.assertEqual({"x": x0, "y": y0, "width": 143, "height": 143}, matches[0]["rect"])
        self.assertGreater(matches[0]["scale"], 1.1)
        self.assertGreaterEqual(matches[0]["match_ratio"], sprite_matcher.FUZZY_MATCH_RATIO_MIN)

    def test_fuzzy_allows_white_source_sprite(self):
        img = np.zeros((64, 64, 4), dtype=np.uint8)
        yy, xx = np.indices((64, 64))
        ring = ((xx - 32) ** 2 + (yy - 32) ** 2 <= 29 ** 2) & ((xx - 32) ** 2 + (yy - 32) ** 2 >= 18 ** 2)
        img[ring] = [48, 48, 48, 255]
        sprite = {
            "path": "C:/fake/GrayBadge.png",
            "rel_path": "GrayBadge.png",
            "img": img,
            "w": 64,
            "h": 64,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "is_white_source": True,
            "white_idx": 0,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.zeros((120, 120, 3), dtype=np.uint8)
        y0, x0 = 18, 26
        scaled = sprite_matcher.resize_sprite(sprite, 70, 70)
        screenshot[y0:y0 + 70, x0:x0 + 70] = scaled[:, :, :3]

        matches = sprite_matcher._match_normal(
            [sprite], [], screenshot, None, [],
            fuzzy_scales=[1.0, 1.1])

        self.assertGreaterEqual(len(matches), 1)
        target = next((m for m in matches if m["rect"] == {"x": x0, "y": y0, "width": 70, "height": 70}), None)
        self.assertIsNotNone(target)
        self.assertTrue(target["is_white"])
        self.assertEqual(0, target["white_idx"])

    def test_build_overlay_renders_fuzzy_matches(self):
        img = np.zeros((16, 16, 4), dtype=np.uint8)
        img[:, :, :3] = [40, 120, 220]
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/FuzzyOnly.png",
            "rel_path": "FuzzyOnly.png",
            "img": img,
            "w": 16,
            "h": 16,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "is_white_source": False,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        overlay = sprite_matcher.build_overlay([
            {
                "sprite_idx": 0,
                "white_idx": -1,
                "is_white": False,
                "is_9slice": False,
                "rect": {"x": 5, "y": 6, "width": 16, "height": 16},
                "fuzzy_match": True,
            }
        ], [sprite], [], 40, 40)

        self.assertGreater(int(np.count_nonzero(overlay[:, :, 3])), 0)

    def test_build_fuzzy_exclusion_mask_marks_overlapping_bbox_only(self):
        mask = sprite_matcher.build_fuzzy_exclusion_mask([
            {
                "fuzzy_match": True,
                "rect": {"x": 12, "y": 13, "width": 8, "height": 6},
            },
            {
                "fuzzy_match": True,
                "rect": {"x": 10, "y": 10, "width": 2, "height": 2},
            },
            {
                "rect": {"x": 10, "y": 10, "width": 20, "height": 20},
            },
        ], 8, 10, 10, 10)

        self.assertEqual((10, 10), mask.shape)
        self.assertEqual(40, int(np.count_nonzero(mask)))
        self.assertTrue(mask[0, 2])
        self.assertTrue(mask[3, 4])
        self.assertTrue(mask[8, 9])
        self.assertFalse(mask[2, 3])

    def test_composite_verify_excludes_fuzzy_bbox_pixels_from_matching(self):
        template = np.full((20, 20, 3), 100, dtype=np.uint8)
        screenshot = template.copy()
        screenshot[:, 10:] = 200
        overlay = np.zeros((20, 20, 4), dtype=np.uint8)
        alpha_mask = np.ones((20, 20), dtype=bool)

        rejected = sprite_matcher.composite_verify(
            template, alpha_mask, 0, 0, 20, 20, overlay, screenshot, max_diff=40)
        accepted = sprite_matcher.composite_verify(
            template, alpha_mask, 0, 0, 20, 20, overlay, screenshot, max_diff=40,
            exclusion_mask=np.pad(np.ones((20, 10), dtype=bool), ((0, 0), (10, 0))))

        self.assertFalse(rejected["accepted"])
        self.assertTrue(accepted["accepted"])
        self.assertEqual(200, accepted["valid_pixels"])

    def test_solid_shape_match_accepts_tinted_foreground_with_contrasting_ring(self):
        alpha = np.zeros((24, 24), dtype=np.uint8)
        alpha[4:20, 7:17] = 255
        alpha[7:17, 4:20] = 255

        screenshot = np.full((48, 48, 3), [20, 30, 160], dtype=np.uint8)
        fg = np.array([235, 253, 254], dtype=np.uint8)
        y0, x0 = 10, 12
        screenshot[y0:y0 + 24, x0:x0 + 24][alpha > 250] = fg

        result = sprite_matcher.verify_solid_shape_match(screenshot, alpha > 250, x0, y0)

        self.assertTrue(result["accepted"])
        self.assertEqual([254, 253, 235], result["tint_color"])
        self.assertEqual(1.0, result["match_ratio"])

    def test_solid_shape_match_rejects_uniform_patch_without_ring_contrast(self):
        alpha = np.zeros((24, 24), dtype=np.uint8)
        alpha[4:20, 7:17] = 255
        alpha[7:17, 4:20] = 255

        screenshot = np.full((48, 48, 3), [235, 253, 254], dtype=np.uint8)
        y0, x0 = 10, 12

        result = sprite_matcher.verify_solid_shape_match(screenshot, alpha > 250, x0, y0)

        self.assertFalse(result["accepted"])

    def test_fuzzy_solid_shape_accepts_inner_occlusion(self):
        alpha = np.zeros((28, 28), dtype=np.uint8)
        alpha[4:24, 4:24] = 255

        screenshot = np.full((56, 56, 3), [24, 32, 48], dtype=np.uint8)
        tint_bgr = np.array([53, 2, 139], dtype=np.uint8)
        y0, x0 = 11, 13
        screenshot[y0:y0 + 28, x0:x0 + 28][alpha > 250] = tint_bgr
        screenshot[y0 + 10:y0 + 18, x0 + 10:x0 + 18] = [235, 253, 254]

        result = sprite_matcher.verify_solid_shape_fuzzy_match(screenshot, alpha > 250, x0, y0)

        self.assertTrue(result["accepted"])
        self.assertEqual("inner_occlusion", result["white_fuzzy_mode"])
        self.assertGreaterEqual(result["edge_match_ratio"], sprite_matcher.WHITE_SOURCE_FUZZY_INNER_EDGE_RATIO_MIN)

    def test_fuzzy_solid_shape_accepts_small_edge_occlusion(self):
        alpha = np.zeros((28, 28), dtype=np.uint8)
        alpha[4:24, 4:24] = 255

        screenshot = np.full((56, 56, 3), [24, 32, 48], dtype=np.uint8)
        tint_bgr = np.array([53, 2, 139], dtype=np.uint8)
        y0, x0 = 11, 13
        screenshot[y0:y0 + 28, x0:x0 + 28][alpha > 250] = tint_bgr
        screenshot[y0 + 4:y0 + 10, x0 + 4:x0 + 8] = [235, 253, 254]

        result = sprite_matcher.verify_solid_shape_fuzzy_match(screenshot, alpha > 250, x0, y0)

        self.assertTrue(result["accepted"])
        self.assertEqual("edge_occlusion", result["white_fuzzy_mode"])
        self.assertGreaterEqual(result["inner_match_ratio"], sprite_matcher.WHITE_SOURCE_FUZZY_EDGE_INNER_RATIO_MIN)

    def test_central_exclusion_hole_rejects_halo_candidate(self):
        alpha = np.zeros((32, 32), dtype=bool)
        alpha[4:28, 4:28] = True
        exclusion = np.zeros((32, 32), dtype=bool)
        exclusion[8:24, 8:24] = True

        self.assertTrue(sprite_matcher._is_central_exclusion_hole(alpha, exclusion))

    def test_composite_solid_shape_accepts_tinted_underlay_with_overlay(self):
        alpha = np.zeros((28, 28), dtype=np.uint8)
        alpha[4:24, 4:24] = 255
        overlay_alpha = np.zeros((28, 28), dtype=np.uint8)
        overlay_alpha[8:20, 12:16] = 255
        overlay_alpha[12:16, 8:20] = 255

        screenshot = np.full((56, 56, 3), [24, 32, 48], dtype=np.uint8)
        tint_bgr = np.array([53, 2, 139], dtype=np.uint8)
        y0, x0 = 11, 13
        screenshot[y0:y0 + 28, x0:x0 + 28][alpha > 250] = tint_bgr
        screenshot[y0:y0 + 28, x0:x0 + 28][overlay_alpha > 0] = [235, 253, 254]

        overlay = np.zeros((56, 56, 4), dtype=np.uint8)
        overlay[y0:y0 + 28, x0:x0 + 28, :3][overlay_alpha > 0] = [235, 253, 254]
        overlay[y0:y0 + 28, x0:x0 + 28, 3][overlay_alpha > 0] = 255

        result = sprite_matcher.composite_verify_solid_shape(alpha > 250, x0, y0, 28, 28, overlay, screenshot)

        self.assertTrue(result["accepted"])
        self.assertEqual([139, 2, 53], result["tint_color"])
        self.assertGreaterEqual(result["match_ratio"], sprite_matcher.WHITE_SOURCE_COMPOSITE_RATIO_MIN)

    def test_composite_solid_shape_rejects_flat_background_without_underlay(self):
        alpha = np.zeros((28, 28), dtype=np.uint8)
        alpha[4:24, 4:24] = 255
        overlay_alpha = np.zeros((28, 28), dtype=np.uint8)
        overlay_alpha[8:20, 12:16] = 255
        overlay_alpha[12:16, 8:20] = 255

        screenshot = np.full((56, 56, 3), [24, 32, 48], dtype=np.uint8)
        y0, x0 = 11, 13
        screenshot[y0:y0 + 28, x0:x0 + 28][overlay_alpha > 0] = [235, 253, 254]

        overlay = np.zeros((56, 56, 4), dtype=np.uint8)
        overlay[y0:y0 + 28, x0:x0 + 28, :3][overlay_alpha > 0] = [235, 253, 254]
        overlay[y0:y0 + 28, x0:x0 + 28, 3][overlay_alpha > 0] = 255

        result = sprite_matcher.composite_verify_solid_shape(alpha > 250, x0, y0, 28, 28, overlay, screenshot)

        self.assertFalse(result["accepted"])

    def test_composite_solid_shape_rejects_multicolor_overlay(self):
        alpha = np.zeros((28, 28), dtype=np.uint8)
        alpha[4:24, 4:24] = 255

        screenshot = np.full((56, 56, 3), [24, 32, 48], dtype=np.uint8)
        screenshot[11:39, 13:41][alpha > 250] = [53, 2, 139]

        overlay = np.zeros((56, 56, 4), dtype=np.uint8)
        overlay[19:31, 25:29, :3] = [255, 255, 255]
        overlay[19:31, 25:29, 3] = 255
        overlay[23:27, 21:33, :3] = [40, 120, 240]
        overlay[23:27, 21:33, 3] = 255
        screenshot[19:31, 25:29] = [255, 255, 255]
        screenshot[23:27, 21:33] = [40, 120, 240]

        result = sprite_matcher.composite_verify_solid_shape(alpha > 250, 13, 11, 28, 28, overlay, screenshot)

        self.assertFalse(result["accepted"])

    def test_white_fuzzy_rejects_large_uniform_region_without_occlusion(self):
        img = np.zeros((20, 20, 4), dtype=np.uint8)
        img[:, :, :3] = [255, 255, 255]
        img[:, :, 3] = 255
        sprite = {
            "path": "C:/fake/WhiteSolid.png",
            "rel_path": "WhiteSolid.png",
            "img": img,
            "w": 20,
            "h": 20,
            "ar": 1.0,
            "border": None,
            "base_sprite_idx": 0,
            "white_idx": 0,
            "is_white_source": True,
        }
        sprite.update(sprite_matcher.compute_sprite_metadata(img))

        screenshot = np.full((20, 20, 3), [32, 48, 200], dtype=np.uint8)

        matches = sprite_matcher._match_white_fuzzy(
            [sprite], screenshot, None, [])

        self.assertEqual([], matches)


if __name__ == "__main__":
    unittest.main()
