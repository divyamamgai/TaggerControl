using System.Drawing;
using System.Windows.Forms;
using TaggerControl;

namespace TaggerControlTest {
    public partial class Main : Form {
        public enum RegexType {
            UserInput,
            Alphabet,
            Digit,
            String
        }
        public string[] TagStringList = { "Detective Conan", "Alphabets", "Digits", "String", "String", "String", "String" };
        public RegexType[] TagList = { RegexType.UserInput, RegexType.Alphabet, RegexType.Digit, RegexType.String, RegexType.String, RegexType.String, RegexType.String };
        public Main() {
            InitializeComponent();
            TaggerFlowPanel TaggerFlowPanelObject = new TaggerFlowPanel(TagStringList, false);
            TaggerFlowPanelObject.Location = new Point(7, 20);
            TaggerFlowPanelObject.Size = new Size(243, 207);
            TaggerFlowPanelObject.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            TaggerGroupBox.Controls.Add(TaggerFlowPanelObject);
        }
    }
}
