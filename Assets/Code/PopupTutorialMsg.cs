using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupTutorialMsg: MonoBehaviour
    {
        public Image img;
        public Text text;
        public Button bDismiss;
        public UIMaster ui;

        public void setTo(int i)
        {

        }

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
