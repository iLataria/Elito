using System;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using Bunker.Core.Services.Base;

namespace Bunker.Core.Services.Fade.Base
{
    public abstract class CoreFadeService : CoreService
    {
        private Image _fadeImg;

        [Inject]
        private void Construct(Image fadeImg)
        {
            _fadeImg = fadeImg;
        }

        private void Awake()
        {
            _fadeImg.color = Color.clear;
            _fadeImg.raycastTarget = false;
        }

        public virtual async UniTask FadeIn(CancellationToken ct, bool inOneFrame = false)
        {
            try
            {
                _fadeImg.raycastTarget = true;

                if (inOneFrame)
                    _fadeImg.color = Color.black;
                //else
                //   // await _fadeImg.DOFade(1, .5f).SetEase(Ease.InOutSine).WithCancellation(ct);

                
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
            }
        }

        public virtual async UniTask FadeOut(CancellationToken ct, bool inOneFrame = false)
        {
            try
            {
                if (inOneFrame)
                    _fadeImg.color = Color.clear;
                //else
                //    await _fadeImg.DOFade(0, .5f).SetEase(Ease.InOutSine).WithCancellation(ct);
                
                _fadeImg.raycastTarget = false;
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
            }
        }
    }
}
