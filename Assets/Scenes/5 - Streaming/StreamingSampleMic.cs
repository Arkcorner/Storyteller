using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;

namespace Whisper.Samples
{
    /// <summary>
    /// Stream transcription from microphone input.
    /// </summary>
    /// TODO: Add Feedback for Things working in the background maybe a throbber, add audio feedback, Translate for German students, Fix UI, Fix bad practices, Implement Mic auto start/stop recording
    public class StreamingSampleMic : MonoBehaviour
    {
        private System.Collections.Generic.HashSet<string> matchedWords = new System.Collections.Generic.HashSet<string>();

        public string[][] StorySegments = new string[][]
        {
            new string[] { "der", "wind", "flog", "durch", "die", "alte", "ruine" },
            new string[] { "the","trees", "cracked", "like", "whispers", "from", "the", "past" },
            new string[] { "a", "thick", "mist", "coiled", "around", "broken", "stones", "and", "forgotten", "pathways", "from", "long", "ago" },
            new string[] { "at", "the", "edge", "stood", "leo", "his", "boots", "worn", "from", "miles", "of", "travel", "determination", "filled", "his", "heart" },
            new string[] { "beyond", "the", "ruins", "lay", "the", "lost", "library", "holding", "knowledge", "that", "could", "change", "everything" },
            new string[] { "he", "stepped", "through", "ivy","covered", "arches", "roots", "wove", "through", "cracked", "stone", "like", "grasping", "fingers" },
            new string[] { "the", "mist", "thickened", "shifting", "like", "a", "living", "thing", "many", "had", "sought", "the", "library", "none", "returned", "the", "same" },
            new string[] { "the", "ruins", "led", "to", "a", "dark", "grove", "where", "an", "ancient", "tree", "stood" },
            new string[] { "its", "twisted", "bark", "and", "sprawling", "roots", "formed", "a", "gate" },
            new string[] { "to", "pass", "one", "had", "to", "give", "a", "truth", "buried", "deep", "within", "the", "heart" },
            new string[] { "leo", "spoke", "his", "truth", "and", "the", "roots", "parted" },
            new string[] { "beyond", "stood", "a", "stone", "door", "covered", "in", "glowing", "symbols" },
            new string[] { "a", "riddle", "was", "carved", "at", "the", "top", "he", "studied", "it", "carefully", "the", "answer", "was", "silence" },
            new string[] { "the", "door", "groaned", "open" },
            new string[] { "inside", "the", "path", "split" },
            new string[] { "to", "the", "left", "a", "golden", "chest", "with", "a", "gleaming", "sword", "atop", "it" },
            new string[] { "to", "the", "right", "an", "old", "book", "on", "a", "pedestal", "its", "cover", "worn", "but", "pulsing", "with", "quiet", "power" },
            new string[] { "he", "made", "his", "choice", "and", "walked", "on" },
            new string[] { "finally", "he", "smelled", "old", "paper" },
            new string[] { "the", "lost", "library", "stood", "before", "him", "its", "great", "doors", "creaking", "open" },
            new string[] { "shelves", "stretched", "endlessly", "into", "shadows" },
            new string[] { "leo", "stepped", "inside" },
            new string[] { "the", "real", "journey", "had", "only", "just", "begun" }
        };
        public string[] Story =
        {
            "Der Wind Flog durch die alte Ruine",
            "The trees cracked like whispers from the past.",
            "A thick mist coiled around broken stones and forgotten pathways from long ago.",
            "At the edge stood Leo, his boots worn from miles of travel. Determination filled his heart.",
            "Beyond the ruins lay the Lost Library, holding knowledge that could change everything.",
            "He stepped through ivy covered arches. Roots wove through cracked stone like grasping fingers.",
            "The mist thickened, shifting like a living thing. Many had sought the library; none returned the same.",
            "The ruins led to a dark grove where an Ancient Tree stood.",
            "Its twisted bark and sprawling roots formed a gate.",
            "To pass, one had to give a truth buried deep within the heart.",
            "Leo spoke his truth and the roots parted.",
            "Beyond stood a Stone Door covered in glowing symbols.",
            "A riddle was carved at the top.He studied it carefully - the answer was silence.",
            "The door groaned open.",
            "Inside, the path split.",
            "To the left, a golden chest with a gleaming sword atop it.",
            "To the right, an old book on a pedestal, its cover worn but pulsing with quiet power.",
            "He made his choice and walked on.",
            "Finally, he smelled old paper.",
            "The Lost Library stood before him, its great doors creaking open.",
            "Shelves stretched endlessly into shadows.",
            "Leo stepped inside.",
            "The real journey had only just begun."
        };
        public string[] Story2 =
        {
            "Der Wind Flog durch die alte Ruine",
            "The trees cracked like whispers from the past.",
            "A thick mist coiled around broken stones and forgotten pathways from long ago.",
            "At the edge stood Leo, his boots worn from miles of travel. Determination filled his heart.",
            "Beyond the ruins lay the Lost Library, holding knowledge that could change everything.",
            "He stepped through ivy covered arches. Roots wove through cracked stone like grasping fingers.",
            "The mist thickened, shifting like a living thing. Many had sought the library; none returned the same.",
            "The ruins led to a dark grove where an Ancient Tree stood.",
            "Its twisted bark and sprawling roots formed a gate.",
            "To pass, one had to give a truth buried deep within the heart.",
            "Leo spoke his truth and the roots parted.",
            "Beyond stood a Stone Door covered in glowing symbols.",
            "A riddle was carved at the top.He studied it carefully - the answer was silence.",
            "The door groaned open.",
            "Inside, the path split.",
            "To the left, a golden chest with a gleaming sword atop it.",
            "To the right, an old book on a pedestal, its cover worn but pulsing with quiet power.",
            "He made his choice and walked on.",
            "Finally, he smelled old paper.",
            "The Lost Library stood before him, its great doors creaking open.",
            "Shelves stretched endlessly into shadows.",
            "Leo stepped inside.",
            "The real journey had only just begun."
        };
        public Color visible = new Color(1f, 1f, 1f, 0f);
        public Color Invisible = new Color(1f, 1f, 1f, 0f);
        public TextDisaperance fadeText;
        public static int CurrentSegment = 0;
        public static bool CanAdvance = false;
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        [Header("UI")]
        public Button button;
        public GameObject SpaceBar;
        public Button button2;
        public Text buttonText;
        public Text text;
        public ScrollRect scroll;
        private WhisperStream _stream;
        private bool awaitingNext = false;

        private async void Update()
        {

            if (!CanAdvance && awaitingNext)
            {
                if (CurrentSegment % 2 == 0)
                {
                    CurrentSegment++;
                    fadeText.StartFadeOut1();
                    fadeText.text2.text = Story[CurrentSegment];
                    fadeText.StartFadeIn2();
                    //Change text in fields back to Actual story
                    print("Appearing 2");
                }
                else
                {
                    CurrentSegment++;
                    print("Appearing 1");
                    fadeText.StartFadeOut2();
                    fadeText.text1.text = Story[CurrentSegment];
                    fadeText.StartFadeIn1();
                }
                awaitingNext = false;
            }
            if (CanAdvance)
            {
                SpaceBar.SetActive(true);
            }
            else
            {
                SpaceBar.SetActive(false);
            }
        }
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
            //Unity is my curse and this Code needing to exist is its proof damned be the Unity Gods
            print("This is the Story" + Story.Length);
            Story = Story;
            print("This is the Story" + Story2.Length);

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
            if (!CanAdvance && !awaitingNext)
            {
                var numbError = 0;
                var processedText = text.ToLower();
                string resultText = "";
                bool allMatched = true;
                print("Text is being compared");
                foreach (string word in StorySegments[CurrentSegment])
                {
                    if (matchedWords.Contains(word.ToLower()))
                    {
                        // If the word was already matched, keep it stricken through
                        resultText += $"<s>{word}</s> ";
                    }
                    else if (processedText.Contains(word))
                    {
                        print("Text has matched" + word);
                        matchedWords.Add(word.ToLower());
                        resultText += $"<s>{word}</s> ";
                    }
                    else
                    {
                        print("Text doesnt Match" + word);
                        numbError++;
                        resultText += $"{word} ";
                        allMatched = false;
                    }
                }
                if (allMatched)
                {
                    print("All words matched! Resetting for the next segment...");
                    matchedWords.Clear(); // Reset matched words list
                    awaitingNext = true;
                }
                // Trim any trailing space
                resultText = resultText.Trim();
                // Assign new text to both Text fields
                fadeText.text1.text = resultText;
                fadeText.text2.text = resultText;
                if (numbError > 0)
                {
                    CanAdvance = false;
                }
                else
                {
                    print("Story can proceed");
                    CanAdvance = true;
                }
            }
        }
    }
}
