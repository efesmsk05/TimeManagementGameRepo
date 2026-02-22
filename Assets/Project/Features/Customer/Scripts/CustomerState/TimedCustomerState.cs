    using System.Threading; // Gerekli
    using Cysharp.Threading.Tasks; // Gerekli
    using UnityEditor;
    using UnityEngine;

    public abstract class TimedCustomerState : CustomerState
    {
        protected float maxDuration;
        protected bool isTimerPaused = false;
        // Görevi iptal etmek için cts
        private CancellationTokenSource _cts;



        public abstract float GetTotalTime(); // ne kadar sürecek
        protected virtual PatienceBarType GetPatienceBarType() { return PatienceBarType.standart; } // hangi bar çalışıcak
        protected abstract void OnTimeout(); // süre bitince ne olucak




        public TimedCustomerState(CustomerController controller) : base(controller)
        {

        }

        public override void Enter()
        {
            _cts = new CancellationTokenSource();

            maxDuration = GetTotalTime();


            if(customerController.currentTable == null)// masdae değilse
            {
                customerController.ShowPatienceBar(GetPatienceBarType(), maxDuration);
                
            }


            TimerRoutine(_cts.Token).Forget();
        }

        public override void Exit()
        {
            customerController.HidePatienceBar();

            if (_cts != null)
            {
                _cts.Cancel(); 
                _cts.Dispose(); 
                _cts = null;
            }
        }

        private async UniTaskVoid TimerRoutine(CancellationToken token)
        {
            while (maxDuration > 0)
            {
                // 1. Güvenlik Kontrolü: İptal edildi mi?
                if (token.IsCancellationRequested) return;

                if (!isTimerPaused)
                {
                    maxDuration -= Time.deltaTime;

                    // UI güncelleme kodların buraya...
                    // customerController.UpdatePatienceUI(maxDuration / GetTotalTime());
                }

                // 2. Bekleme sırasında iptal edilirse, await'ten hemen çıkması için token veriyoruz
                // SuppressCancellationThrow: İptal edilince hata fırlatmasın, sessizce çıksın.
                bool isCancelled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token).SuppressCancellationThrow();
                
                // Eğer bekleme sırasında iptal edildiyse (Exit çağrıldıysa) döngüyü kır ve çık
                if (isCancelled) return;
            }

            // Eğer buraya geldiyse süre bitmiştir VE iptal edilmemiştir.
            OnTimeout();
        }



    }