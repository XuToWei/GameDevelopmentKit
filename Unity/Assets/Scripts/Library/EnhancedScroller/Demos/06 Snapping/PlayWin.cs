using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EnhancedScrollerDemos.SnappingDemo
{
    /// <summary>
    /// This class displays a popup panel when the user wins some credits
    /// </summary>
    public class PlayWin : MonoBehaviour
    {
        /// <summary>
        /// Cached transform to speed up processing
        /// </summary>
        private Transform _transform;

        /// <summary>
        /// The amount of time left before moving to the next state
        /// </summary>
        private float _timeLeft;

        /// <summary>
        /// This is the UI text element to show the score
        /// </summary>
        public Text scoreText;

        /// <summary>
        /// The amount of time to zoom in
        /// </summary>
        public float zoomTime;

        /// <summary>
        /// The amount of time to hold in place
        /// </summary>
        public float holdTime;

        /// <summary>
        /// The amount of time to disappear
        /// </summary>
        public float unZoomTime;

        void Awake()
        {
            // cache the transform and hide the panel
            _transform = transform;
            _transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// This function intiates the playing of the panel
        /// </summary>
        /// <param name="score"></param>
        public void Play(int score)
        {
            // set the score text
            scoreText.text = string.Format("{0:n0}", score);

            // hide the panel to start
            transform.localScale = Vector3.zero;

            // reset the timer to the zoom time and start the play zoom
            _timeLeft = zoomTime;
            StartCoroutine(PlayZoom());
        }

        /// <summary>
        /// This function makes the panel larger over time
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayZoom()
        {
            while (_timeLeft > 0)
            {
                // decrement the timer
                _timeLeft -= Time.deltaTime;

                // set the scale of the transform between hidden and showing based on the amount of time left
                _transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (zoomTime - _timeLeft) / zoomTime);

                yield return null;
            }

            // set the transform to full showing (in case our Lerp didn't quite finish at 1)
            transform.localScale = Vector3.one;

            // reset the timer to the hold time and play the hold
            _timeLeft = holdTime;
            StartCoroutine(PlayHold());
        }

        /// <summary>
        /// This function waits for a set amount of time
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayHold()
        {
            while (_timeLeft > 0)
            {
                // decrement the timer
                _timeLeft -= Time.deltaTime;
                yield return null;
            }

            // reset the timer to the unzoom time and play the unzoom
            _timeLeft = unZoomTime;
            StartCoroutine(PlayUnZoom());
        }

        /// <summary>
        /// This function hides the panel over time
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayUnZoom()
        {
            while (_timeLeft > 0)
            {
                // decrement the timer
                _timeLeft -= Time.deltaTime;

                // set the scale of the transform between showing and hidden based on the amount of time left
                _transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (unZoomTime - _timeLeft) / unZoomTime);

                yield return null;
            }

            // set the transform to full hidden (in case our Lerp didn't quite finish at 0)
            transform.localScale = Vector3.zero;
        }
    }
}