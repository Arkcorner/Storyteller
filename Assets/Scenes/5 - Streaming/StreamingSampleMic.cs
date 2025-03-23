using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;

namespace Whisper.Samples
{
    /// <summary>
    /// Stream transcription from microphone input.
    /// </summary>
    public class StreamingSampleMic : MonoBehaviour
    {
        public TextDisaperance fadeText;
        public static int CurrentSegment = 0;
        public static bool CanAdvance = false;
        public string[] StorySegments0 = { "hello", "world" };
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        [Header("UI")]
        public Button button;
        public Button button2;
        public Text buttonText;
        public Text text;
        public ScrollRect scroll;
        private WhisperStream _stream;

        private async void Start()
        {
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;
            _stream.OnSegmentUpdated += OnSegmentUpdated;
            _stream.OnSegmentFinished += OnSegmentFinished;
            _stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
            button.onClick.AddListener(OnButtonPressed);
            button2.onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                _stream.StartStream();
                microphoneRecord.StartRecord();
            }
            else
                microphoneRecord.StopRecord();
        }

        private void OnRecordStop(AudioChunk recordedAudio)
        {
        }

        private void OnResult(string result)
        {
            text.text = result;
            UiUtils.ScrollDown(scroll);
        }

        private void OnSegmentUpdated(WhisperResult segment)
        {
            print($"Segment updated: {segment.Result}");
        }

        private void OnSegmentFinished(WhisperResult segment)
        {
            print($"Segment finished: {segment.Result}");
            MatchText(segment.Result);
        }

        private void OnFinished(string finalResult)
        {
            print("Stream finished!");
        }
        private void MatchText(string text)
        {
            var numbError = 0;
            var processedText = text.ToLower();
            print("Text is being compared");
            for (int n = 0; n < StorySegments0.Length; n++)
            {
                if (processedText.Contains(StorySegments0[n]))
                {
                    print("Text has matched" + StorySegments0[n]);
                }
                else
                {
                    print("Text doesnt Match");
                    numbError++;
                }

            }
            if (numbError > 0)
            {
                CanAdvance = false;
            }
            else
            {
                print("Story can proceed");
                CurrentSegment++;
                if (CurrentSegment % 2 == 0)
                {
                    CanAdvance = true;
                    fadeText.StartFadeOut1();
                    fadeText.StartFadeIn2();
                    print("Appearing 2");
                }
                else
                {
                    print("Appearing 1");
                    CanAdvance = true;
                    fadeText.StartFadeOut2();
                    fadeText.StartFadeIn1();
                }
            }
        }
    }
}
