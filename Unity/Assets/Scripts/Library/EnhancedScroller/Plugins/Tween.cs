using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;
using System.Collections;

namespace EnhancedUI.EnhancedScroller
{
    public class Tween : MonoBehaviour
    {
        /// <summary>
        /// The easing type
        /// </summary>
        public enum TweenType
        {
            immediate,
            linear,
            spring,
            easeInQuad,
            easeOutQuad,
            easeInOutQuad,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeInQuart,
            easeOutQuart,
            easeInOutQuart,
            easeInQuint,
            easeOutQuint,
            easeInOutQuint,
            easeInSine,
            easeOutSine,
            easeInOutSine,
            easeInExpo,
            easeOutExpo,
            easeInOutExpo,
            easeInCirc,
            easeOutCirc,
            easeInOutCirc,
            easeInBounce,
            easeOutBounce,
            easeInOutBounce,
            easeInBack,
            easeOutBack,
            easeInOutBack,
            easeInElastic,
            easeOutElastic,
            easeInOutElastic
        }

        private float _tweenTimeLeft;

        /// <summary>
        /// Moves the scroll position over time between two points given an easing function. When the
        /// tween is complete it will fire the jumpComplete delegate.
        /// </summary>
        /// <param name="tweenType">The type of easing to use</param>
        /// <param name="time">The amount of time to interpolate</param>
        /// <param name="start">The starting scroll position</param>
        /// <param name="end">The ending scroll position</param>
        /// <param name="jumpComplete">The action to fire when the tween is complete</param>
        /// <returns></returns>
        public IEnumerator TweenPosition(TweenType tweenType, float time, float start, float end, Action<float, float> tweenUpdated, Action tweenComplete)
        {
            if (!(tweenType == TweenType.immediate || time == 0))
            {
                _tweenTimeLeft = 0;
                var newPosition = 0f;
                var lastPosition = start;

                // while the tween has time left, use an easing function
                while (_tweenTimeLeft < time)
                {
                    switch (tweenType)
                    {
                        case TweenType.linear: newPosition = linear(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.spring: newPosition = spring(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInQuad: newPosition = easeInQuad(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutQuad: newPosition = easeOutQuad(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutQuad: newPosition = easeInOutQuad(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInCubic: newPosition = easeInCubic(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutCubic: newPosition = easeOutCubic(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutCubic: newPosition = easeInOutCubic(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInQuart: newPosition = easeInQuart(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutQuart: newPosition = easeOutQuart(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutQuart: newPosition = easeInOutQuart(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInQuint: newPosition = easeInQuint(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutQuint: newPosition = easeOutQuint(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutQuint: newPosition = easeInOutQuint(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInSine: newPosition = easeInSine(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutSine: newPosition = easeOutSine(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutSine: newPosition = easeInOutSine(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInExpo: newPosition = easeInExpo(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutExpo: newPosition = easeOutExpo(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutExpo: newPosition = easeInOutExpo(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInCirc: newPosition = easeInCirc(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutCirc: newPosition = easeOutCirc(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutCirc: newPosition = easeInOutCirc(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInBounce: newPosition = easeInBounce(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutBounce: newPosition = easeOutBounce(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutBounce: newPosition = easeInOutBounce(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInBack: newPosition = easeInBack(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutBack: newPosition = easeOutBack(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutBack: newPosition = easeInOutBack(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInElastic: newPosition = easeInElastic(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeOutElastic: newPosition = easeOutElastic(start, end, (_tweenTimeLeft / time)); break;
                        case TweenType.easeInOutElastic: newPosition = easeInOutElastic(start, end, (_tweenTimeLeft / time)); break;
                    }

                    if (tweenUpdated != null) tweenUpdated(newPosition, newPosition - lastPosition);

                    lastPosition = newPosition;

                    // increase the time elapsed
                    _tweenTimeLeft += Time.unscaledDeltaTime;

                    yield return null;
                }
            }

            // the time has expired, so we make sure the final value
            // is the actual end position.
            if (tweenUpdated != null) tweenUpdated(end, 0);

            // the tween jump is complete, so we fire the delegate
            if (tweenComplete != null) tweenComplete();
        }

        private float linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        private static float spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + (1.2f * (1f - val)));
            return start + (end - start) * val;
        }

        private static float easeInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        private static float easeOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        private static float easeInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        private static float easeInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        private static float easeOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        private static float easeInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        private static float easeInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        private static float easeOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        private static float easeInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        private static float easeInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        private static float easeOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        private static float easeInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        private static float easeInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float easeOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        private static float easeInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        private static float easeInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        private static float easeOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        private static float easeInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        private static float easeInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        private static float easeOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        private static float easeInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        private static float easeInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - val) + start;
        }

        private static float easeOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }

        private static float easeInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return easeInBounce(0, end, val * 2) * 0.5f + start;
            else return easeOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        private static float easeInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        private static float easeOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        private static float easeInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        private static float easeInElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        private static float easeOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        private static float easeInOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / (d / 2);
            if (val == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }

    }
}
