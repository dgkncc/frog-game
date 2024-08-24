using FrogGame._Core.Managers;
using UnityEngine;

namespace FrogGame._Core.Common
{
    public abstract class PoolMonoBehaviour : MonoBehaviour
    {
        public virtual void ReturnToPool()
        {
            PoolManager.Instance?.ReturnObjectToPool(this);
        }
        public virtual void OnInitialized() { }

        public virtual void OnGotFromPool() { }

        public virtual void OnReturnedToPool() { }

    }
}
