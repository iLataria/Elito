using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Cysharp.Threading.Tasks;
using System.Threading;
using Bunker.Core.Views.Base;

namespace Bunker.Core.Services.GameLoading.Base
{
    public class CoreGameLoadingServiceView : CoreView
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _loadingDesc;
        [SerializeField] private TextMeshProUGUI _loadingText;

        public Slider ProgressBar => _progressBar;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public TextMeshProUGUI LoadingDesc => _loadingDesc;
        public TextMeshProUGUI LoadingText => _loadingText;

        private void Awake()
        {
            _progressBar.value = 0f;
            _canvasGroup.alpha = 0f;
            _loadingText.text = string.Empty;
        }

        public override UniTask Show(CancellationToken ct, bool needShowAnimation = true)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
    }
}
