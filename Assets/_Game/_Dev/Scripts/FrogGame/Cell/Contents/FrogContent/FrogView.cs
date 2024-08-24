using DG.Tweening;
using FrogGame._Core.Common;
using FrogGame.ScriptableObjects;
using UnityEngine;

namespace FrogGame.Cell.Contents.FrogContent
{

    public class FrogView : PoolMonoBehaviour
    {
        [SerializeField] private FrogViewModel _viewModel;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private GameObject models;
        [SerializeField] private Animation frogAnimations;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            SubscribeEventActions();
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                UnsubscribeEventActions();
            }
        }

        private void SubscribeEventActions()
        {
            _viewModel.OnContentUpdated += UpdateView;
            _viewModel.OnActivateContent += ActivateModels;
            _viewModel.OnDeactivateContent += DeactivateModels;
            _viewModel.OnEatingStart += PlayOpenMouthAnimation;
            _viewModel.OnEatingEnd += PlayCloseMouthAnimation;
        }

        private void UnsubscribeEventActions()
        {
            _viewModel.OnContentUpdated -= UpdateView;
            _viewModel.OnActivateContent -= ActivateModels;
            _viewModel.OnDeactivateContent -= DeactivateModels;
            _viewModel.OnEatingStart -= PlayOpenMouthAnimation;
            _viewModel.OnEatingEnd -= PlayCloseMouthAnimation;
        }

        public void ReceiveInput() => _viewModel.ReceiveInput();

        private void UpdateView()
        {
            SetFrogMaterial();
            SetFrogDirection();
        }

        private void PlayOpenMouthAnimation()
        {
            frogAnimations.Play("FrogMouthOpen");

        }

        private void PlayCloseMouthAnimation()
        {
            frogAnimations.Play("FrogMouthClose");
        }

        private void ActivateModels()
        {
            models.SetActive(true);
            models.transform.DOScale(GlobalConstants.CONTENT_SCALED_UP_VECTOR, GlobalConstants.TWEEN_DURATION_IDEAL);
        }

        private void DeactivateModels()
        {
            models.SetActive(false);
            models.transform.localScale = GlobalConstants.CONTENT_SCALED_DOWN_VECTOR;
        }

        private void SetFrogMaterial()
        {
            var material = GameSettings.Instance.GetFrogMaterial(_viewModel.ContentColor);
            skinnedMeshRenderer.material = material;
        }

        private void SetFrogDirection()
        {
            var rotation = GameSettings.Instance.GetDirectionRotation(_viewModel.ContentDirection);
            transform.eulerAngles = rotation;
        }
    }


}
