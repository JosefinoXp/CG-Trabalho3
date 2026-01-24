using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace LCDModule
{
    /// <summary>
    /// Controls the LCD display behavior, including animation sequences, 
    /// date/time display, and sensor data monitoring.
    /// </summary>
    public class LCDController : MonoBehaviour
    {
        public LCDModule LCD; // Reference to the LCD display component

        [Header("Display Mode")]
        [Tooltip("Display mode: 1=Plain, 2=Demo, 3=Date/Time, 4=External Reference")]
        [Range(1, 4)]
        public int mode = 4;

        private int lastMode = 0;   // lastMode

        // ---------- Animation control ----------
        private int animIndex = 0;          // Current animation step index
        private float elapsedTime = 0f;     // Elapsed time in seconds
        private bool firstStepFlg = false;  // True on the first frame of each step

        // List of animation steps: each has an Action and duration (in seconds)
        private List<(Action action, float durationSec)> initSequence;

        // ---------- Monitoring ----------
        public SensorDataProvider dataProvider; // Data provider reference

        // ---------- Other ----------
        // 'ÿ' represents a filled block (used in progress/gauge visualization)
        // 'Ę' represents the degree symbol (°) used for temperature display

        void Start()
        {
            // Display frame rate settings
            // (Should not be modified in distributable packages)
            QualitySettings.vSyncCount = 0; // Disable VSync
            Application.targetFrameRate = 60; // Target 60 FPS
            // -------------------------------------

            // Initialize animation sequence
            initSequence = new List<(Action, float)>()
            {
                (Step_01, 2.0f), (Step_02, 4.0f), (Step_03, 1.0f), (Step_04, 1.0f), (Step_05, 1.0f),
                (Step_06, 1.0f), (Step_07, 1.0f), (Step_08, 2.5f), (Step_09, 1.0f), (Step_10, 1.0f),
                (Step_11, 1.0f), (Step_12, 1.0f), (Step_13, 1.0f), (Step_14, 1.0f), (Step_15, 1.0f),
                (Step_16, 1.0f), (Step_17, 1.0f), (Step_18, 1.0f), (Step_19, 1.0f),
            };

            // Initialize step flag
            firstStepFlg = true;

            // Subscribe to data change events
            dataProvider.OnDataChanged += OnSensorDataChanged;

        }

        /// <summary>
        /// Called once per frame.
        /// Updates display content according to the current mode.
        /// </summary>
        void Update()
        {
            switch (mode)
            {
                case 1:
                    UpdateMode1();
                    break;
                case 2:
                    UpdateMode2();
                    break;
                case 3:
                    UpdateMode3();
                    break;
                case 4:
                    UpdateMode4();
                    break;

            }

            // 
            lastMode = mode;

        }

        /// <summary>
        /// Mode 1: Static text display.
        /// </summary>
        private void UpdateMode1()
        {
            LCD.setSTR("This is LCD module", "Plain Text Display");

        }

        /// <summary>
        /// Mode 2: Sequential animation demo.
        /// </summary>
        private void UpdateMode2()
        {
            if (animIndex >= initSequence.Count)
            {
                // 終了時の挙動（ループまたは停止）
                animIndex = 0; // ループする
                return;
            }

            elapsedTime += Time.deltaTime; // フレームに依存しない時間加算

            // 現在のステップ処理を実行
            initSequence[animIndex].action.Invoke();

            // ステップ経過時間を超えたら次へ
            if (elapsedTime >= initSequence[animIndex].durationSec)
            {
                animIndex++;
                elapsedTime = 0f;
                firstStepFlg = true;
            }
        }

        /// <summary>
        /// Mode 3: Real-time date/time display.
        /// </summary>
        private void UpdateMode3()
        {
            var culture = new CultureInfo("en-US");
            string dateStr = DateTime.Now.ToString("yyyy/MM/dd (ddd)", culture);
            string timeStr = DateTime.Now.ToString("HH:mm:ss");
            LCD.setSTR(dateStr, timeStr);
        }

        /// <summary>
        /// Mode 4: External data reference display (e.g., temperature/humidity).
        /// </summary>
        private void UpdateMode4()
        {
            if (lastMode != mode)
            {
                dataProvider.temperature = 0;
            }
        }

        /// <summary>
        /// Calculates substring display progression based on elapsed time and speed.
        /// </summary>
        private string CalcStringToDisplay(string Str, float eTime, int speed)
        {
            return Str.Substring(0, Math.Clamp((int)(eTime * speed), 0, Str.Length));
        }

        // ---------- Animation Step Methods (Use in Mode 3) ----------
        private void Step_01()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("", "");
            }
        }

        private void Step_02()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ", "ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ");
            }

            float alpha = Mathf.Lerp(0f, 100f, elapsedTime / initSequence[animIndex].durationSec * 1.25f);
            LCD.SetAlpha((int)alpha);
        }
        private void Step_03()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.SetAlpha(100);
                LCD.setSTR("INITIALIZE", "");
            }
        }

        private void Step_04()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("INITIALIZE.", "");
            }
        }
        private void Step_05()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("INITIALIZE..", "");
            }
        }
        private void Step_06()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("INITIALIZE...", "");
            }
        }
        private void Step_07()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("INITIALIZE...OK", "");
            }
        }

        private void Step_08()
        {
            LCD.setSTR(null, CalcStringToDisplay("LOADING MODULES", elapsedTime, 40));
        }


        private void Step_09()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("DisplayController", elapsedTime, 40));
        }
        private void Step_10()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "DisplayController OK");
            }
        }
        private void Step_11()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("Texture Bitmap", elapsedTime, 40));
        }
        private void Step_12()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "Texture Bitmap OK");
            }
        }
        private void Step_13()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("SpriteCreater", elapsedTime, 40));
        }
        private void Step_14()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOADING MODULES", "SpriteCreater OK");
            }
        }
        private void Step_15()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("SpriteCreater OK", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("LOAD MODULES SUCCESS", elapsedTime, 40));
        }
        private void Step_16()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOAD MODULES SUCCESS", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("CHECKSUM", elapsedTime, 40));
        }
        private void Step_17()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("LOAD MODULES SUCCESS", "CHECKSUM OK");
            }
        }
        private void Step_18()
        {
            if (firstStepFlg == true)
            {
                firstStepFlg = false;
                LCD.setSTR("CHECKSUM OK", "");
            }

            LCD.setSTR(null, CalcStringToDisplay("SYSTEM READY", elapsedTime, 40));
        }
        private void Step_19()
        {
            // wait
        }

        /// <summary>
        /// Called when temperature or humidity values change.  
        /// Updates the LCD display with the current readings.  
        /// Displays temperature numerically and humidity as a bar gauge.
        /// </summary>
        /// <param name="temp">Current temperature value (in Celsius).</param>
        /// <param name="hum">Current humidity value (in percentage).</param>
        private void OnSensorDataChanged(float temp, float hum)
        {
            // Format the first LCD line with the temperature value (e.g., "TEMP:  25.3 °C")
            string line1 = $"TEMP: {temp,5:0.0} ĘC";

            // Generate a humidity gauge (total of 14 blocks)
            int totalBlocks = 14; // Maximum number of blocks representing 100% humidity
            int filledBlocks = Mathf.Clamp(Mathf.RoundToInt(hum / (100 / totalBlocks)), 0, totalBlocks); // Number of filled blocks
            string gauge = new string('ÿ', filledBlocks); // Create a string with filled blocks

            // Format the second LCD line with the humidity gauge (e.g., "HUMI: ÿÿÿÿÿ")
            string line2 = $"HUMI: {gauge}";

            LCD.setSTR(line1, line2);  // Send updated strings to LCD
        }
    }
}