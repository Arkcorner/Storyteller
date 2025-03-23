using System;
using Unity.VisualScripting;
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
        public string[][] StorySegments = new string[][]
   {
        new string[] { "the", "wind", "howled", "through", "the", "ancient", "ruins" },
        new string[] { "leaves", "rustled", "like", "whispers", "from", "the", "past" },
        new string[] { "trees", "loomed", "overhead" },
        new string[] { "their", "gnarled", "branches", "clawed", "at", "the", "sky" },
        new string[] { "a", "thick", "mist", "coiled", "around", "the", "ground" },
        new string[] { "it", "hid", "broken", "stones", "and", "forgotten", "pathways", "from", "long", "ago" },
        new string[] { "at", "the", "edge", "of", "the", "ruins", "stood", "a", "boy", "named", "leo" },
        new string[] { "his", "journey", "had", "been", "long" },
        new string[] { "his", "boots", "were", "worn", "from", "miles", "of", "travel" },
        new string[] { "determination", "filled", "his", "heart" },
        new string[] { "beyond", "the", "ruins", "lay", "the", "lost", "library" },
        new string[] { "it", "held", "knowledge", "long", "forgotten" },
        new string[] { "it", "could", "change", "everything" },
        new string[] { "he", "stepped", "through", "ivy-covered", "arches" },
        new string[] { "crumbling", "walls", "surrounded", "him" },
        new string[] { "the", "path", "beneath", "his", "feet", "was", "cracked" },
        new string[] { "roots", "wove", "through", "the", "stone", "like", "grasping", "fingers" },
        new string[] { "the", "air", "grew", "heavy", "with", "silence" },
        new string[] { "the", "mist", "thickened", "shifting", "like", "a", "living", "thing" },
        new string[] { "many", "had", "tried", "to", "reach", "the", "library" },
        new string[] { "none", "had", "ever", "returned", "the", "same" },
        new string[] { "leo", "stepped", "inside" },
        new string[] { "the", "world", "around", "him", "hushed" },
        new string[] { "the", "real", "journey", "had", "only", "just", "begun" }
   };
        public string[] Story =
        {
        "The wind howled through the ancient ruins.",
        "Leaves rustled like whispers from the past.",
        "Trees loomed overhead.",
        "Their gnarled branches clawed at the sky.",
        "A thick mist coiled around the ground.",
        "It hid broken stones and forgotten pathways from long ago.",
        "At the edge of the ruins stood a boy named Leo.",
        "His journey had been long.",
        "His boots were worn from miles of travel.",
        "Determination filled his heart.",
        "Beyond the ruins lay the Lost Library.",
        "It held knowledge long forgotten.",
        "It could change everything.",
        "He stepped through ivy-covered arches.",
        "Crumbling walls surrounded him.",
        "The path beneath his feet was cracked.",
        "Roots wove through the stone like grasping fingers.",
        "The air grew heavy with silence.",
        "The mist thickened, shifting like a living thing.",
        "Many had tried to reach the library.",
        "None had ever returned the same.",
        "Leo stepped inside.",
        "The world around him hushed.",
        "The real journey had only just begun."
        };
        public Color visible = new Color(1f, 1f, 1f, 1f);
        public Color Invisible = new Color(1f, 1f, 1f, 0f);
        public TextDisaperance fadeText;
        public static int CurrentSegment = 0;
        public static bool CanAdvance = false;
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        [Header("UI")]
        public Button button;
        public SpriteRenderer Walkable;
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

            fadeText.text1.text = Story[0];
            fadeText.text2.text = Story[1];
        }
        private void Update()
        {
            if (CanAdvance)
            {
                Walkable.color = Invisible;
            }
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
            foreach (string word in StorySegments[CurrentSegment])
            {
                if (processedText.Contains(word))
                {
                    print("Text has matched" + word);
                }
                else
                {
                    print("Text doesnt Match" + word);
                    numbError++;
                }
            }
            if (numbError > 0)
            {
                CanAdvance = false;
                Walkable.color = Invisible;
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
                    Walkable.color = visible;
                    print("Appearing 2");
                }
                else
                {
                    print("Appearing 1");
                    CanAdvance = true;
                    fadeText.StartFadeOut2();
                    fadeText.StartFadeIn1();
                    fadeText.text1.text = Story[CurrentSegment];
                    Walkable.color = visible;
                    fadeText.text2.text = Story[CurrentSegment + 1];
                }
            }
        }
    }
}
