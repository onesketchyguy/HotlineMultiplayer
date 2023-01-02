using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class SwitchPicker : MonoBehaviour
    {
        public string[] descriptions;
        public Sprite[] images;

        public Text descDisplay;
        public Image imageDisplay;

        public int index;

        public void CycleRight()
        {
            index++;
            if (index > images.Length - 1) index = 0;
            UpdateDisplay();
        }

        public void CycleLeft()
        {
            index--;
            if (index < 0) index = images.Length - 1;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (descDisplay != null) descDisplay.text = descriptions[index];
            if (imageDisplay != null) imageDisplay.sprite = images[index];
        }

        private void Start()
        {
            UpdateDisplay();
        }
    }
}
