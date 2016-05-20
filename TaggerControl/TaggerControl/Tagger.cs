using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaggerControl {

    public struct TaggerFlags {
        public bool Initialized, Hover, Selected, CrossHover, CrossSelected;
    }

    public class TaggerTheme {

        public class TaggerThemeElement {
            public SolidBrush NormalBackground, HoverBackground;
            public Pen NormalBorder, HoverBorder;
            public TaggerThemeElement(SolidBrush NormalBackground, SolidBrush HoverBackground, Pen NormalBorder, Pen HoverBorder) {
                this.NormalBackground = NormalBackground;
                this.HoverBackground = HoverBackground;
                this.NormalBorder = NormalBorder;
                this.HoverBorder = HoverBorder;
            }
        }

        public TaggerThemeElement Normal, Selected;
        public SolidBrush FontColor, CrossNormalBackground, CrossHoverBackground, CrossSelectedBackground;

        public TaggerTheme(TaggerThemeElement Normal, TaggerThemeElement Selected, SolidBrush FontColor,
            SolidBrush CrossNormalBackground, SolidBrush CrossHoverBackground, SolidBrush CrossSelectedBackground) {
            this.Normal = Normal;
            this.Selected = Selected;
            this.FontColor = FontColor;
            this.CrossNormalBackground = CrossNormalBackground;
            this.CrossHoverBackground = CrossHoverBackground;
            this.CrossSelectedBackground = CrossSelectedBackground;
        }

    }

    public static class TaggerThemeContainer {

        public static TaggerTheme Light = new TaggerTheme(
                new TaggerTheme.TaggerThemeElement(
                    new SolidBrush(Color.FromArgb(144, 202, 249)), new SolidBrush(Color.FromArgb(187, 222, 251)),
                    new Pen(new SolidBrush(Color.FromArgb(25, 118, 210)), 2), new Pen(new SolidBrush(Color.FromArgb(66, 165, 245)), 2)),
                new TaggerTheme.TaggerThemeElement(
                    new SolidBrush(Color.FromArgb(156, 204, 101)), new SolidBrush(Color.FromArgb(174, 213, 129)),
                    new Pen(new SolidBrush(Color.FromArgb(85, 139, 47)), 2), new Pen(new SolidBrush(Color.FromArgb(124, 179, 66)), 2)),
                new SolidBrush(Color.FromArgb(0, 0, 0)),
                new SolidBrush(Color.FromArgb(220, 198, 40, 40)),
                new SolidBrush(Color.FromArgb(255, 229, 57, 53)),
                new SolidBrush(Color.FromArgb(255, 183, 28, 28))
            );

        public static TaggerTheme Dark = new TaggerTheme(
                new TaggerTheme.TaggerThemeElement(
                    new SolidBrush(Color.FromArgb(40, 53, 147)), new SolidBrush(Color.FromArgb(63, 81, 181)),
                    new Pen(new SolidBrush(Color.FromArgb(26, 35, 126)), 2), new Pen(new SolidBrush(Color.FromArgb(40, 53, 147)), 2)),
                new TaggerTheme.TaggerThemeElement(
                    new SolidBrush(Color.FromArgb(0, 105, 92)), new SolidBrush(Color.FromArgb(0, 150, 136)),
                    new Pen(new SolidBrush(Color.FromArgb(0, 77, 64)), 2), new Pen(new SolidBrush(Color.FromArgb(0, 105, 92)), 2)),
                new SolidBrush(Color.FromArgb(255, 255, 255)),
                new SolidBrush(Color.FromArgb(173, 20, 87)),
                new SolidBrush(Color.FromArgb(233, 30, 99)),
                new SolidBrush(Color.FromArgb(136, 14, 79))
            );

    }

    public partial class Tagger : Control {

        private const int MaxLength = 255;
        private int Border, TwiceBorder;
        private bool EditingValue = false;
        private Size CalculatedSize;
        private Rectangle BorderRectangle, CrossRectangle, CrossOverlayRectangle;
        private TextBox EditTextBox;

        private int IDValue;
        public int ID {
            get { return IDValue; }
            set { IDValue = value; }
        }

        private bool CrossFillOnTopValue;
        public bool CrossFillOnTop {
            get { return CrossFillOnTopValue; }
            set { CrossFillOnTopValue = value; }
        }

        private bool AllowEditValue;
        public bool AllowEdit {
            get { return AllowEditValue; }
            set { AllowEditValue = value; }
        }

        private TaggerFlags FlagsValue;
        public TaggerFlags Flags {
            get { return FlagsValue; }
        }

        private MouseEventHandler OnCrossClickValue = DefaultCrossClickEvent;
        public MouseEventHandler OnCrossClick {
            get { return OnCrossClickValue; }
            set { OnCrossClickValue = value; }
        }

        private TaggerTheme ThemeValue = TaggerThemeContainer.Light;
        public TaggerTheme Theme {
            get { return ThemeValue; }
            set {
                ThemeValue = value;
                Border = (int)ThemeValue.Normal.NormalBorder.Width;
                TwiceBorder = Border << 1;
                EditTextBox.ForeColor = Theme.FontColor.Color;
                InitializeDimensions();
            }
        }

        public override string Text {
            get { return base.Text; }
            set {
                if (value.Length <= MaxLength) {
                    base.Text = value;
                } else {
                    base.Text = value.Substring(0, MaxLength);
                }
            }
        }

        public Tagger(int id) {
            Initialize();
            IDValue = id;
            InitializeComponent();
        }

        public Tagger() {
            Initialize();
            IDValue = new Random().Next();
            InitializeComponent();
        }

        private void Initialize() {
            // This is needed if you want to set the Size according to the text.
            Width = 1;
            Height = 1;
            FlagsValue.Hover = false;
            FlagsValue.Selected = false;
            FlagsValue.Initialized = false;
            CrossFillOnTopValue = false;
            AllowEditValue = true;
            Padding = new Padding(3, 4, 3, 3);
            Font = new Font(new FontFamily("Segoe UI"), 8);
            EditTextBox = new TextBox();
            EditTextBox.MaxLength = MaxLength;
            EditTextBox.BorderStyle = BorderStyle.None;
            EditTextBox.KeyDown += EditTextBoxKeyDown;
            EditTextBox.LostFocus += EditTextBoxLostFocus;
            EditTextBox.Visible = false;
            EditTextBox.Enabled = false;
            EditTextBox.CausesValidation = false;
            Border = (int)ThemeValue.Normal.NormalBorder.Width;
            TwiceBorder = Border << 1;
            EditTextBox.ForeColor = Theme.FontColor.Color;
            Controls.Add(EditTextBox);
        }

        private void InitializeCalculatedSize(ref PaintEventArgs pe) {
            CalculatedSize = pe.Graphics.MeasureString(Text, Font).ToSize();
            InitializeDimensions();
        }

        private void InitializeDimensions() {
            if (!CalculatedSize.IsEmpty) {
                CrossRectangle.Width = CrossRectangle.Height = (int)Math.Round(CalculatedSize.Height * 0.75);
                CalculatedSize.Height += Padding.Vertical;
                Height = CalculatedSize.Height;
                CrossOverlayRectangle.Width = CrossOverlayRectangle.Height = Height;
                CalculatedSize.Width += Padding.Horizontal + TwiceBorder;
                CrossOverlayRectangle.X = CalculatedSize.Width;
                CalculatedSize.Width += CrossOverlayRectangle.Width;
                Width = CalculatedSize.Width;
                CrossRectangle.Y = (Height - CrossRectangle.Width) >> 1;
                CrossRectangle.X = CrossOverlayRectangle.X + CrossRectangle.Y;
                BorderRectangle = new Rectangle(1, 1, Width - Border, Height - Border);
                EditTextBox.Location = new Point(Padding.Left + Border, Padding.Top);
                EditTextBox.Size = new Size(CrossOverlayRectangle.X - EditTextBox.Location.X, Height);
            }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            // No need to initialize again and again.
            if (!FlagsValue.Initialized) {
                InitializeCalculatedSize(ref pe);
                FlagsValue.Initialized = true;
            }
            pe.Graphics.FillRectangle(FlagsValue.Selected ? FlagsValue.Hover ? ThemeValue.Selected.HoverBackground : ThemeValue.Selected.NormalBackground : FlagsValue.Hover ? ThemeValue.Normal.HoverBackground : ThemeValue.Normal.NormalBackground, 0, 0, Width, Height);
            pe.Graphics.DrawRectangle(FlagsValue.Selected ? FlagsValue.Hover ? ThemeValue.Selected.HoverBorder : ThemeValue.Selected.NormalBorder : FlagsValue.Hover ? ThemeValue.Normal.HoverBorder : ThemeValue.Normal.NormalBorder, BorderRectangle);
            pe.Graphics.DrawString(Text, Font, ThemeValue.FontColor, Padding.Left, Padding.Top);
            if (CrossFillOnTopValue) {
                pe.Graphics.DrawImage(Properties.Resources.CrossRed, CrossRectangle);
                pe.Graphics.FillRectangle(FlagsValue.CrossSelected ? ThemeValue.CrossSelectedBackground : FlagsValue.Hover ? FlagsValue.CrossHover ? ThemeValue.CrossHoverBackground : ThemeValue.CrossNormalBackground : ThemeValue.CrossNormalBackground, CrossOverlayRectangle);
            } else {
                pe.Graphics.FillRectangle(FlagsValue.CrossSelected ? ThemeValue.CrossSelectedBackground : FlagsValue.Hover ? FlagsValue.CrossHover ? ThemeValue.CrossHoverBackground : ThemeValue.CrossNormalBackground : ThemeValue.CrossNormalBackground, CrossOverlayRectangle);
                pe.Graphics.DrawImage(Properties.Resources.CrossRed, CrossRectangle);
            }
            if (FlagsValue.CrossSelected) FlagsValue.CrossSelected = false;
            base.OnPaint(pe);
        }

        protected override void OnMouseEnter(EventArgs e) {
            FlagsValue.Hover = true;
            if (FlagsValue.Selected) EditTextBox.BackColor = ThemeValue.Selected.HoverBackground.Color;
            else EditTextBox.BackColor = ThemeValue.Normal.HoverBackground.Color;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            FlagsValue.Hover = false;
            if (FlagsValue.Selected) EditTextBox.BackColor = ThemeValue.Selected.NormalBackground.Color;
            else EditTextBox.BackColor = ThemeValue.Normal.NormalBackground.Color;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.X >= CrossOverlayRectangle.X) {
                if (!FlagsValue.CrossHover) {
                    FlagsValue.CrossHover = true;
                    Invalidate();
                }
            } else if (FlagsValue.CrossHover) {
                FlagsValue.CrossHover = false;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            Focus();
            if (FlagsValue.CrossHover) {
                FlagsValue.CrossSelected = !FlagsValue.CrossSelected;
                if (EditingValue) ExitEditing();
                else OnCrossClickValue?.Invoke(this, e);
            } else {
                FlagsValue.Selected = !FlagsValue.Selected;
                EditTextBox.BackColor = FlagsValue.Selected ? ThemeValue.Selected.NormalBackground.Color : ThemeValue.Normal.NormalBackground.Color;
                base.OnMouseClick(e);
            }
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            if (AllowEditValue) Edit();
            base.OnMouseDoubleClick(e);
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnGotFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.KeyCode == Keys.Return) {
                if (AllowEditValue) {
                    if (EditingValue) DoneEditing();
                    else Edit();
                }
            }
            base.OnKeyDown(e);
        }

        public void SelectTagger() {
            Focus();
            if (!FlagsValue.Selected) {
                FlagsValue.Selected = true;
                EditTextBox.BackColor = FlagsValue.Hover ? ThemeValue.Selected.HoverBackground.Color : ThemeValue.Selected.NormalBackground.Color;
                Invalidate();
            }
        }

        public void DeselectTagger() {
            if (FlagsValue.Selected) {
                FlagsValue.Selected = false;
                EditTextBox.BackColor = FlagsValue.Hover ? ThemeValue.Normal.HoverBackground.Color : ThemeValue.Normal.NormalBackground.Color;
                Invalidate();
            }
        }

        public static void DefaultCrossClickEvent(object tagger, MouseEventArgs e) {
            Tagger TaggerObject = (Tagger)tagger;
            DialogResult PerformRemove = MessageBox.Show("Do you really want to remove " + TaggerObject.Text + " tag?", "Remove Tag", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (PerformRemove == DialogResult.Yes) {
                TaggerObject.Dispose();
            }
        }

        public void Edit() {
            EditingValue = true;
            SelectTagger();
            EditTextBox.Text = Text;
            EditTextBox.Visible = true;
            EditTextBox.Enabled = true;
            EditTextBox.Focus();
        }

        public void DoneEditing() {
            if (EditTextBox.Text.Length > 0) {
                Text = EditTextBox.Text;
                EditTextBox.Visible = false;
                EditTextBox.Enabled = false;
                FlagsValue.Initialized = false;
                EditingValue = false;
                Invalidate();
            } else {
                ExitEditing();
            }
        }

        public void ExitEditing() {
            EditTextBox.Visible = false;
            EditTextBox.Enabled = false;
            EditingValue = false;
        }

        private void EditTextBoxKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return) {
                DoneEditing();
            }
        }

        private void EditTextBoxLostFocus(object sender, EventArgs e) {
            if (!Focused)
                ExitEditing();
        }

    }

    public partial class TaggerFlowPanel : FlowLayoutPanel {

        private bool MultiSelectValue = false;
        public bool MultiSelect {
            get { return MultiSelectValue; }
            set {
                MultiSelectValue = value;
                ChangeMultiSelect();
            }
        }

        private Tagger LastSelectedTagger = null;

        private List<Tagger> TaggerListContainer;
        public List<Tagger> TaggerList {
            get { return TaggerListContainer; }
        }


        private TaggerTheme ThemeValue = TaggerThemeContainer.Light;
        public TaggerTheme Theme {
            get { return ThemeValue; }
            set {
                ThemeValue = value;
                ChangeTheme();
            }
        }

        public TaggerFlowPanel(List<Tagger> taggerList, bool multiSelect = false) {
            MultiSelectValue = multiSelect;
            TaggerListContainer = new List<Tagger>();
            foreach (Tagger TaggerObject in taggerList) {
                Controls.Add(TaggerObject);
            }
        }

        public TaggerFlowPanel(List<string> taggerTextList, bool multiSelect = false) {
            int Index = 0, Count = taggerTextList.Count;
            Tagger TaggerObject;
            MultiSelectValue = multiSelect;
            TaggerListContainer = new List<Tagger>();
            for (; Index < Count; Index++) {
                TaggerObject = new Tagger(Index);
                TaggerObject.Text = taggerTextList[Index];
                Controls.Add(TaggerObject);
            }
        }

        public TaggerFlowPanel(string[] taggerTextArray, bool multiSelect = false) {
            int Index = 0, Length = taggerTextArray.GetLength(0);
            Tagger TaggerObject;
            MultiSelectValue = multiSelect;
            TaggerListContainer = new List<Tagger>();
            for (; Index < Length; Index++) {
                TaggerObject = new Tagger(Index);
                TaggerObject.Text = taggerTextArray[Index];
                Controls.Add(TaggerObject);
            }
        }

        public void ShiftSelectedRight() {
            int LastIndex = TaggerListContainer.Count - 1, Index = LastIndex, NextIndex;
            Tagger TaggerSelectedObject, TaggerNextObject;
            for (; Index >= 0; Index--) {
                TaggerSelectedObject = TaggerListContainer[Index];
                if (TaggerSelectedObject.Flags.Selected) {
                    if (Index < LastIndex) {
                        NextIndex = Index + 1;
                        TaggerNextObject = TaggerListContainer[NextIndex];
                        if (!TaggerNextObject.Flags.Selected) {
                            Controls.SetChildIndex(TaggerSelectedObject, NextIndex);
                            Controls.SetChildIndex(TaggerNextObject, Index);
                            TaggerListContainer[Index] = TaggerNextObject;
                            TaggerListContainer[NextIndex] = TaggerSelectedObject;
                        }
                    }
                }
            }
        }

        public void ShiftSelectedLeft() {
            int Index = 1, Count = TaggerListContainer.Count, PreviousIndex;
            Tagger TaggerSelectedObject, TaggerPreviousObject;
            for (; Index < Count; Index++) {
                TaggerSelectedObject = TaggerListContainer[Index];
                if (TaggerSelectedObject.Flags.Selected) {
                    PreviousIndex = Index - 1;
                    TaggerPreviousObject = TaggerListContainer[PreviousIndex];
                    if (!TaggerPreviousObject.Flags.Selected) {
                        Controls.SetChildIndex(TaggerSelectedObject, PreviousIndex);
                        Controls.SetChildIndex(TaggerPreviousObject, Index);
                        TaggerListContainer[Index] = TaggerPreviousObject;
                        TaggerListContainer[PreviousIndex] = TaggerSelectedObject;
                    }
                }
            }
        }

        private void ChangeMultiSelect() {
            int Index = 0, Count = TaggerListContainer.Count;
            if (MultiSelectValue) {
                for (; Index < Count; Index++) {
                    TaggerListContainer[Index].MouseClick += null;
                }
            } else {
                for (; Index < Count; Index++) {
                    TaggerListContainer[Index].MouseClick += DeselectLastSelected;
                }
            }
        }

        private void ChangeTheme() {
            int Index = 0, Count = TaggerListContainer.Count;
            for (; Index < Count; Index++) {
                TaggerListContainer[Index].Theme = ThemeValue;
            }
        }

        protected void DeselectLastSelected(object tagger, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Tagger TaggerObject = (Tagger)tagger;
                if (TaggerObject.Flags.Selected) {
                    if (LastSelectedTagger != null && LastSelectedTagger.ID != TaggerObject.ID) {
                        LastSelectedTagger.DeselectTagger();
                    }
                    LastSelectedTagger = TaggerObject;
                }
            }
        }

        protected void TaggerKeyDown(object tagger, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Left:
                    ShiftSelectedLeft();
                    break;
                case Keys.Right:
                    ShiftSelectedRight();
                    break;
            }
        }

        protected override void OnControlAdded(ControlEventArgs e) {
            if (e.Control is Tagger) {
                Tagger TaggerObject = (Tagger)e.Control;
                TaggerObject.MouseClick += MultiSelectValue ? (MouseEventHandler)null : DeselectLastSelected;
                TaggerObject.KeyUp += TaggerKeyDown;
                TaggerObject.Theme = ThemeValue;
                TaggerListContainer.Add(TaggerObject);
            } else {
                Controls.Remove(e.Control);
            }
            base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e) {
            TaggerListContainer.Remove((Tagger)e.Control);
            base.OnControlRemoved(e);
        }

    }

}
