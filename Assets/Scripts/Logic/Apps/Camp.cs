using System.Collections;
using UnityEngine;

namespace Kaisa.Digivice.App {
    public class Camp : DigiviceApp {

        private SpriteBuilder sbCamp;
        private Coroutine animCamp;

        private Sprite[] PlayerSprites => gm.spriteDB.GetCharacterSprites(gm.PlayerChar);

        public override void InputA() {
            EndCamp();
        }

        protected override void StartApp() {
            gm.EnqueueAnimation(Animations.OpenCamp(PlayerSprites));
            sbCamp = ScreenElement.BuildSprite("Camp", Parent).SetSize(24, 24).Center().SetSprite(gm.spriteDB.camp[0]);
            animCamp = StartCoroutine(PAnimateCamp());
        }

        private void EndCamp() {
            gm.EnqueueAnimation(Animations.CloseCamp(PlayerSprites));
            gm.EnqueueAnimation(Animations.CharHappy());
            gm.isCharacterDefeated = false;
            StopCoroutine(animCamp);
            CloseApp(Screen.Character);
        }

        private IEnumerator PAnimateCamp() {
            yield return new WaitForSeconds(7.5f);
            bool altSprite = true;
            while(true) {
                if(altSprite) {
                    sbCamp.SetSprite(gm.spriteDB.camp[1]);
                    altSprite = false;
                    yield return new WaitForSeconds(0.5f);
                }
                else {
                    sbCamp.SetSprite(gm.spriteDB.camp[0]);
                    altSprite = true;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

}