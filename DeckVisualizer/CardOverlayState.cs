using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckVisualizer
{
    public struct CardOverlayState
    {
        public Color WashColor { get; set; }
        public string Label { get; set; }

        public CardOverlayState(Color washColor, string label)
        {
            WashColor = washColor;
            Label = label;
        }

        public static readonly List<CardOverlayState> OverlayCycleList = new List<CardOverlayState>
        {
            new CardOverlayState(Color.Transparent, ""),
            new CardOverlayState(Color.FromArgb(110, Color.LimeGreen), "Field"),
            new CardOverlayState(Color.FromArgb(110, Color.Red), "Discard"),
            new CardOverlayState(Color.FromArgb(110, Color.Orange), "Hand"),
            new CardOverlayState(Color.FromArgb(110, Color.DeepSkyBlue), "Deck"), 
            new CardOverlayState(Color.FromArgb(110, Color.GhostWhite), "Facedown")
        };
    }
}
