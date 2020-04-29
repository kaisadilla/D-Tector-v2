using UnityEngine;

//TODO: Credits missing!
namespace Kaisa.Digivice {
    public class ShakeDetector : MonoBehaviour {
        private GameManager gm;

        private int nextStep;
        float accelUpdateInterval = 1f / 60f;
        // The greater the value of LowPassKernelWidthInSeconds, the slower the
        // filtered value will converge towards current input sample (and vice versa).
        float lowPassKernelWidthInSeconds = 1.0f;
        // This next parameter is initialized to 2.0 per Apple's recommendation,
        // or at least according to Brady! ;)
        float shakeDetectionThreshold = 2.0f;

        float lowPassFilterFactor;
        Vector3 lowPassValue;

        int steps = 0;
        int timeIdle = 0;

        public void AssignManagers(GameManager gm) {
            this.gm = gm;
        }

        void Start() {
            lowPassFilterFactor = accelUpdateInterval / lowPassKernelWidthInSeconds;
            shakeDetectionThreshold *= shakeDetectionThreshold;
            lowPassValue = Input.acceleration;
        }

        void Update() {
            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - lowPassValue;

            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold) {
                if (nextStep < 4) {
                    nextStep++;
                    return;
                }

                nextStep = 0;
                steps++;
                gm.TakeAStep();
                gm.playerChar.currentState = CharState.Walking;

                //sm.PlaySound_ButtonA();
            }
            else {
                if (timeIdle < 150) {
                    timeIdle++;
                    return;
                }
                timeIdle = 0;
                gm.playerChar.currentState = CharState.Idle;
            }
        }
    }
}