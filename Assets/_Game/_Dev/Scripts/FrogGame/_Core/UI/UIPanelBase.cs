using UnityEngine;

namespace FrogGame._Core.UI
{
    public class UIPanelBase : MonoBehaviour
    {
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
		
		public virtual void Close()
		{
			gameObject.SetActive(false);
		}
		
    }
}