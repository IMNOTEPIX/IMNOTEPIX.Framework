using System.Collections;

namespace IMNOTEPIX.Framework.fCore
{
    public class AfterAbortClass : GTA.Script
    {
        public AfterAbortClass()
        {
            AfterAbort.Register("FadeIn", FadeIn);
            Tick += (s, e) => AfterAbort.Process();
            Aborted += AfterAbortRequest;
        }

        private void AfterAbortRequest(object sender, System.EventArgs e)
        {
            AfterAbort.Request("FadeIn");
        }

        private IEnumerator FadeIn()
        {
            if (GTA.UI.Screen.IsFadedOut || GTA.UI.Screen.IsFadingOut)
                GTA.UI.Screen.FadeIn(500);
            yield return AfterAbort.WaitFor(0);
        }
    }
}
