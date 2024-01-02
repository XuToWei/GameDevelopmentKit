using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;

namespace EnhancedScrollerDemos.SnappingDemo
{
    /// <summary>
    /// This demo shows how you can use snapping to lock a slot cell to a particular
    /// spot in the scroller when the scroller's velocity dips below a certain 
    /// threshhold. The demo script controls three separate slot controller scripts,
    /// though we could have just as easily done each of their logic in this one script.
    /// </summary>
    public class SnappingDemo : MonoBehaviour
    {
        /// <summary>
        /// The states the game can be in
        /// </summary>
        private enum GameStateEnum
        {
            Initializing,
            Playing,
            GameOver
        }

        /// <summary>
        /// Cached array of the slot controllers
        /// </summary>
        private SlotController[] _slotControllers;

        /// <summary>
        /// The data index value of each scroll controller
        /// </summary>
        private int[] _snappedDataIndices;

        /// <summary>
        /// The current credits available for play
        /// </summary>
        private int _credits;

        /// <summary>
        /// The number of scrollers that have snapped and stopped
        /// during this lever pull. Once the count reaches three,
        /// the score will be tallied.
        /// </summary>
        private int _snapCount;

        /// <summary>
        /// The current state of the game
        /// </summary>
        private GameStateEnum _gameState;

        /// <summary>
        /// The minimum and maximum values that each scroller
        /// can have when the lever is pulled. This script
        /// will pick a random value between the minimum and
        /// maximum and also randomly pick the direction of movement.
        /// </summary>
        public float minVelocity;
        public float maxVelocity;

        /// <summary>
        /// These are the indices of the various slots,
        /// used in tallying the score
        /// </summary>
        public int cherryIndex;
        public int sevenIndex;
        public int tripleBarIndex;
        public int bigWinIndex;
        public int blankIndex;

        /// <summary>
        /// These are the sprites for the slot data. We
        /// could have placed these in the SlotController script,
        /// but then we'd need to duplicate the list three times.
        /// </summary>
        public Sprite[] slotSprites;

        /// <summary>
        /// This is the button used to start the slots moving
        /// </summary>
        public Button pullLeverButton;

        /// <summary>
        /// This shows how many credits are left
        /// </summary>
        public Text creditsText;

        /// <summary>
        /// The number of credits to start with
        /// </summary>
        public int startingCredits;

        /// <summary>
        /// Panels and gameobjects used in the game states
        /// </summary>
        public GameObject playingPanel;
        public GameObject gameOverPanel;
        public PlayWin playWin;

        /// <summary>
        /// Setting the credits will alter some UI
        /// </summary>
        private int Credits
        {
            get
            {
                return _credits;
            }
            set
            {
                // don't let the credits dip below zero
                _credits = (value < 0 ? 0 : value);

                // show the credits
                creditsText.text = string.Format("{0:n0}", _credits);

                // deactivate the lever if the credits are zero
                pullLeverButton.gameObject.SetActive(_credits > 0);
            }
        }

        /// <summary>
        /// Setting the game state will alter the game's UI
        /// </summary>
        private GameStateEnum GameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;

                switch (_gameState)
                {
                    case GameStateEnum.Playing:

                        // turn on the snapping for each scroller in the slot controllers
                        foreach (var slotController in _slotControllers)
                        {
                            slotController.scroller.snapping = true;
                        }

                        // reset the credits and activate the playing panel,
                        // deactivating the game over panel

                        Credits = startingCredits;

                        playingPanel.SetActive(true);
                        gameOverPanel.SetActive(false);

                        break;

                    case GameStateEnum.GameOver:

                        // activate the game over panel and deactivate
                        // the playing panel

                        playingPanel.SetActive(false);
                        gameOverPanel.SetActive(true);

                        break;
                }
            }
        }

        void Awake()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // set our game to initializing
            GameState = GameStateEnum.Initializing;

            // cache the slot controllers from this game object
            _slotControllers = gameObject.GetComponentsInChildren<SlotController>();

            // create array for the selected data indices arrays
            _snappedDataIndices = new int[_slotControllers.Length];

            // set each slot controller's scroll snapping handler
            // (we could have done this in the slot controller script, 
            // but handling it here gives us information about the
            // game's state and overall values for each slot controller)
            foreach (var slotController in _slotControllers)
            {
                slotController.scroller.scrollerSnapped = ScrollerSnapped;
            }
        }

        void Start()
        {
            // reload each scroll controller with sprites for the data
            foreach (var slotController in _slotControllers)
            {
                slotController.Reload(slotSprites);
            }
        }

        void LateUpdate()
        {
            // We set the playing state here so that the game
            // doesn't show a winner when it starts up (because
            // there will always be three of a kind at startup). 
            if (GameState == GameStateEnum.Initializing)
                GameState = GameStateEnum.Playing;
        }

        /// <summary>
        /// This function is linked in the editor to the Pull Lever Button's click event
        /// </summary>
        public void PullLeverButton_OnClick()
        {
            // reset the snap count back to zero. When the count reaches three,
            // the score will be tallied
            _snapCount = 0;

            // take away a credit to play
            Credits--;

            // disable the lever button
            pullLeverButton.interactable = false;

            // loop through each slot controller and add a random velocity and direction to the scrollers
            foreach (var slotController in _slotControllers)
            {
                slotController.AddVelocity((UnityEngine.Random.Range(0, 1f) > 0.5f ? 1f : -1f) * UnityEngine.Random.Range(minVelocity, maxVelocity));
            }
        }

        /// <summary>
        /// This function is linked in the editor to the Reset Button's Click event
        /// </summary>
        public void ResetButton_OnClick()
        {
            // reset the game back to playing from a game over state
            GameState = GameStateEnum.Playing;
        }

        /// <summary>
        /// This is the handler of each snapping in the scroller. The 
        /// cell index will only be different from the data index if looping is on.
        /// </summary>
        /// <param name="scroller">The EnhancedScroller that fired the event</param>
        /// <param name="cellIndex">The index of the cell that snapped on</param>
        /// <param name="dataIndex">The data index of the cell that snapped on</param>
		private void ScrollerSnapped(EnhancedScroller scroller, int cellIndex, int dataIndex, EnhancedScrollerCellView cellView)
        {
            // if we are not playing, ignore this event
            if (GameState != GameStateEnum.Playing) return;

            // increment the snap count. We will need three total snaps to tally the score
            _snapCount++;

            // set the slot of the snapped scroller for use in tallying the score
            _snappedDataIndices[_snapCount - 1] = dataIndex;

            if (_snapCount == _slotControllers.Length)
            {
                // if we've reached the final snap count, then tally the score 
                TallyScore();

                // reenable the lever
                pullLeverButton.interactable = true;
            }

            if (Credits == 0)
            {
                // if we are out of credits after tallying the score, go to a game over state
                GameState = GameStateEnum.GameOver;
            }
        }

        /// <summary>
        /// This function calculates the slot score and updates the credits if necessary
        /// </summary>
        private void TallyScore()
        {
            // reset the snap count
            _snapCount = 0;

            var score = 0;

            // get shortcuts to the snapped slots
            var s1 = _snappedDataIndices[0];
            var s2 = _snappedDataIndices[1];
            var s3 = _snappedDataIndices[2];

            if (s1 == blankIndex || s2 == blankIndex || s3 == blankIndex)
            {
                // at least one of the slot is blank, so no score
                score = 0;
            }
            else
            {
                if (s1 == s2 && s1 == s3)
                {
                    // all three slots are equal

                    if (s1 == sevenIndex)
                    {
                        // jackpot!
                        score = 1000;
                    }
                    else if (s1 == bigWinIndex)
                    {
                        // three big win 
                        score = 150;
                    }
                    else if (s1 == tripleBarIndex)
                    {
                        // three triple bar
                        score = 70;
                    }
                    else if (s1 == cherryIndex)
                    {
                        // three cherries
                        score = 40;
                    }
                    else
                    {
                        // three of something else
                        score = 20;
                    }
                }
                else if (s1 == cherryIndex || s2 == cherryIndex || s3 == cherryIndex)
                {
                    // at least one cherry showed up
                    score = 3;
                }
            }

            if (score > 0)
            {
                // we had a score, so we add it to the credits and show the play win panel
                Credits += score;
                playWin.Play(score);
            }
        }
    }
}