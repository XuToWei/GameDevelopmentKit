using UnityEngine;
using System.Collections;

namespace Slate
{

    [ExecuteInEditMode]
    ///<summary>Forwards Animator based calls</summary>
    public class AnimatorDispatcher : MonoBehaviour
    {

        public event System.Action<int> onAnimatorIK;

        private Animator _animator;
        private Animator animator {
            get { return _animator != null ? _animator : _animator = GetComponent<Animator>(); }
        }

        void OnAnimatorIK(int index) {
            if ( onAnimatorIK != null ) {
                onAnimatorIK(index);
            }
        }
    }
}