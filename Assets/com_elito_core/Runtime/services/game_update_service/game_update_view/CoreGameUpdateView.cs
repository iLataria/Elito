using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Bunker.Core.Signals;
using Bunker.Core.Views.Base;
using Bunker.Core.Services.GameLoading.Base;
using Bunker.Core.Utilities;

namespace Bunker.Core.Views.UpdateGame
{
    public abstract class CoreGameUpdateView : CoreView
    {
        private CoreGameLoadingService _model;

        [SerializeField] protected Button _okBtn;

        [Inject]
        private void Construct(CoreGameLoadingService model, [Inject (Id = "click_blocker")] RectTransform clickBlocker)
        {
            _model = model;
            _clickBlocker = clickBlocker;
        }

        private void Start()
        {
            Assert.SerializedFields(this);
            _ = Hide(destroyCancellationToken, needCloseAnimation: false);
            _okBtn.onClick.AddListener(UpdateGameOkBtnClicked);
        }

        private void UpdateGameOkBtnClicked()
        {
            _clickBlocker.gameObject.SetActive(true);
            _signalBus.Fire(new StartUpdateGameSignal());
        }
    }
}
