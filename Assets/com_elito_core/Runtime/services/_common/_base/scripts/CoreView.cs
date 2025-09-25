using System;
using System.Threading;

using UnityEngine;

using UniRx;
using Zenject;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

using Bunker.Core.Utilities;
using Bunker.Core.Tools.Debug.Base;

using static Bunker.Core.Utilities.Utils;

namespace Bunker.Core.Views.Base
{
    public abstract class CoreView : MonoBehaviour, IDisposable
    {
        protected Animator _anim;
        protected SignalBus _signalBus;
        private bool _inOpenCloseProcess;
        protected CoreDebugTool _debugTool;
        protected RectTransform _clickBlocker;
        protected CompositeDisposable _disposables = new();

        [ValueDropdown(nameof(GetAnimatorStateItems), NumberOfItemsBeforeEnablingSearch = 5, SortDropdownItems = true)]
        [SerializeField]
        AnimationStateProperty openProp = new AnimationStateProperty { name = "Open" };

        [ValueDropdown(nameof(GetAnimatorStateItems), NumberOfItemsBeforeEnablingSearch = 5, SortDropdownItems = true)]
        [SerializeField]
        AnimationStateProperty openInstantProp = new AnimationStateProperty { name = "OpenInstant" };

        [ValueDropdown(nameof(GetAnimatorStateItems), NumberOfItemsBeforeEnablingSearch = 5, SortDropdownItems = true)]
        [SerializeField]
        AnimationStateProperty closeProp = new AnimationStateProperty { name = "Close" };

        [ValueDropdown(nameof(GetAnimatorStateItems), NumberOfItemsBeforeEnablingSearch = 5, SortDropdownItems = true)]
        [SerializeField]
        AnimationStateProperty closeInstantProp = new AnimationStateProperty { name = "CloseInstant" };

        [SerializeField]
        [Min(0f)]
        private float closeTime = 0.5f;

        public bool useUnscaledTime = true;

        [Inject]
        private void Construct(CoreDebugTool debugTool, SignalBus signalBus, [Inject(Id = "view_main_animator")] Animator anim)
        {
            _anim = anim;
            _signalBus = signalBus;
            _debugTool = debugTool;
        }

        private void Awake()
        {
            Utilities.Assert.SerializedFields(this);
            _clickBlocker?.gameObject.SetActive(true);
        }

        public virtual async UniTask Show(CancellationToken ct, bool needShowAnimation = true)
        {
            if (gameObject.activeSelf || _inOpenCloseProcess)
                return;

            gameObject.SetActive(true);
            int animationNameHash = needShowAnimation ? openProp.NameHash() : openInstantProp.NameHash();
            _anim.Play(animationNameHash);

            _inOpenCloseProcess = true;
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !_anim.IsInTransition(0), cancellationToken: ct);
            _inOpenCloseProcess = false;
            _clickBlocker?.gameObject.SetActive(false);
        }

        public virtual async UniTask Hide(CancellationToken ct, bool needCloseAnimation = true)
        {
            if (!gameObject.activeSelf || _inOpenCloseProcess)
                return;

            _clickBlocker?.gameObject.SetActive(true);

            int animationNameHash = needCloseAnimation ? closeProp.NameHash() : closeInstantProp.NameHash();
            _anim.Play(animationNameHash);

            _inOpenCloseProcess = true;
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !_anim.IsInTransition(0), cancellationToken: ct);
            _inOpenCloseProcess = false;
            gameObject.SetActive(false);
        }

        public virtual async UniTask Toggle(CancellationToken ct)
        {
            if (gameObject.activeSelf)
                await Hide(ct);
            else
                await Show(ct);
        }

        public virtual async UniTask Close(CancellationToken ct)
        {
            await Hide(ct);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private ValueDropdownList<AnimationStateProperty> GetAnimatorStateItems()
        {
            Animator animator = GetComponent<Animator>();
            return Utils.GetAnimatorStateItems(animator);
        }

        public virtual void Dispose()
        {
            _disposables.Dispose();
            _debugTool.Log($"Dispose {GetType()}");
        }
    }
}
