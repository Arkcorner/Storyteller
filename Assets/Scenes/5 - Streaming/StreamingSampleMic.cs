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

        public string[][] StorySegments = new string[][] {
            new string[] { "es", "war", "einmal", "ein", "mutiger", "junge", "namens", "leo", "der", "die", "verlorene", "bibliothek", "mit", "ihrem", "uralten", "wissen", "suchte" },
            new string[] { "er", "wanderte", "durch", "neblige", "ruinen", "bis", "zu", "einem", "dunklen", "hain" },
            new string[] { "dort", "stand", "ein", "uralter", "baum", "dessen", "wurzeln", "ein", "tor", "bildeten" },
            new string[] { "sag", "eine", "wahrheit", "aus", "deinem", "herzen", "sprach", "der", "baum" },
            new string[] { "leo", "antwortete", "ich", "suche", "wissen", "nicht", "macht" },
            new string[] { "die", "wurzeln", "öffneten", "sich" },
            new string[] { "dahinter", "war", "eine", "steinerne", "tür", "mit", "einem", "rätsel" },
            new string[] { "leo", "dachte", "nach", "und", "schwieg", "das", "war", "die", "antwort" },
            new string[] { "die", "tür", "öffnete", "sich" },
            new string[] { "drinnen", "teilte", "sich", "der", "weg" },
            new string[] { "links", "eine", "goldene", "truhe", "mit", "einem", "glänzenden", "schwert" },
            new string[] { "rechts", "ein", "altes", "buch", "das", "sanft", "leuchtete" },
            new string[] { "leo", "erinnerte", "sich", "an", "seine", "wahrheit", "und", "wählte", "das", "buch" },
            new string[] { "das", "buch", "führte", "ihn", "zur", "bibliothek" },
            new string[] { "die", "großen", "türen", "öffneten", "sich" },
            new string[] { "leo", "trat", "ein", "sein", "abenteuer", "mit", "dem", "wahren", "schatz", "dem", "wissen", "begann" },
            new string[] { "und", "wenn", "er", "nicht", "gestorben", "ist", "dann", "lernt", "er", "noch", "heute", "aus", "den", "büchern", "der", "magischen", "bibliothek" }
        };
        public string[] Story = {
            "Es war einmal ein mutiger Junge namens Leo, der die Verlorene Bibliothek mit ihrem uralten Wissen suchte.",
            "Er wanderte durch neblige Ruinen bis zu einem dunklen Hain.",
            "Dort stand ein uralter Baum, dessen Wurzeln ein Tor bildeten.",
            "Sag eine Wahrheit aus deinem Herzen, sprach der Baum.",
            "Leo antwortete: Ich suche Wissen, nicht Macht.",
            "Die Wurzeln öffneten sich.",
            "Dahinter war eine steinerne Tür mit einem Rätsel.",
            "Leo dachte nach und schwieg - das war die Antwort.",
            "Die Tür öffnete sich.",
            "Drinnen teilte sich der Weg.",
            "Links eine goldene Truhe mit einem glänzenden Schwert.",
            "Rechts ein altes Buch, das sanft leuchtete.",
            "Leo erinnerte sich an seine Wahrheit und wählte das Buch.",
            "Das Buch führte ihn zur Bibliothek.",
            "Die großen Türen öffneten sich.",
            "Leo trat ein - sein Abenteuer mit dem wahren Schatz, dem Wissen, begann.",
            "Und wenn er nicht gestorben ist, dann lernt er noch heute aus den Büchern der magischen Bibliothek."
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
                        print("Actual "+processedText);
                        print("Expected " + word);
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
