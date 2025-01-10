using System;
using System.Diagnostics;
using NUnit.Framework;

namespace LineAdjustment.Tests
{
    public class LineAdjustmentAlgorithmTests
    {
        [Test]
        [TestCase(null, 5, "")]
        [TestCase("", 5, "")]
        [TestCase("test", 5, "test ")]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            "Lorem  ipsum\ndolor    sit\namet        \nconsectetur \nadipiscing  \nelit  sed do\neiusmod     \ntempor      \nincididunt  \nut labore et\ndolore magna\naliqua      ")]
        [TestCase("Lorem     ipsum    dolor", 17, "Lorem ipsum dolor")]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 120,
            "Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua")]
        public void Simple(string input, int lineWidth, string expected)
        {
            expected = FixNewLines(expected);
            
            var algorithm = new LineAdjustmentAlgorithm();
            var output = algorithm.Transform(input, lineWidth);
            Assert.AreEqual(expected, output);
        }

        [TestCase("VeryVeryLongWord Is Here", 10, "VeryVeryLo\nngWord  Is\nHere      ")]
        [TestCase("VeryVeryLongWordPlusPlus Is Here", 10, "VeryVeryLo\nngWordPlus\nPlus    Is\nHere      ")]
        [TestCase("VeryVeryLongWordPlusPlus AnotherLongWord Is Here", 10, 
            "VeryVeryLo\nngWordPlus\nPlus      \nAnotherLon\ngWord   Is\nHere      ")]
        public void NormalizeWordList(string input, int lineWidth, string expected)
        {
            expected = FixNewLines(expected);
            
            var algorithm = new LineAdjustmentAlgorithm();
            var output = algorithm.Transform(input, lineWidth);
            Assert.AreEqual(expected, output);
        }
        
        [TestCase("A B С", 8, "A   B  С")]
        public void ComposeLineAndAppend(string input, int lineWidth, string expected)
        {
            expected = FixNewLines(expected);
            
            var algorithm = new LineAdjustmentAlgorithm();
            var output = algorithm.Transform(input, lineWidth);
            Assert.AreEqual(expected, output);
        }
        
        [Test]
        public void ThrowsArgumentExecptions()
        {
            var algorithm = new LineAdjustmentAlgorithm();
            
            Assert.Throws<ArgumentException>(() => algorithm.Transform("", 0));
        }

        [Test]
        public void AlwaysSuccess_JustPrintResultToDebug()
        {
            var input = "Sed ut perspiciatis, unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam eaque ipsa, quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt, explicabo. Nemo enim ipsam voluptatem, quia voluptas sit, aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos, qui ratione voluptatem sequi nesciunt, neque porro quisquam est, qui dolorem ipsum, quia dolor sit, amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt, ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit, qui in ea voluptate velit esse, quam nihil molestiae consequatur, vel illum, qui dolorem eum fugiat, quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus, qui blanditiis praesentium voluptatum deleniti atque corrupti, quos dolores et quas molestias excepturi sint, obcaecati cupiditate non provident, similique sunt in culpa, qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio, cumque nihil impedit, quo minus id, quod maxime placeat, facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet, ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat. ";
            
            var algorithm = new LineAdjustmentAlgorithm();
            var result = algorithm.Transform(input, 30);
            
            Debug.Write(result);
            
            // Sed   ut   perspiciatis,  unde
            // omnis  iste  natus  error  sit
            // voluptatem         accusantium
            // doloremque  laudantium,  totam
            // rem  aperiam  eaque ipsa, quae
            // ab illo inventore veritatis et
            // quasi  architecto beatae vitae
            // dicta  sunt,  explicabo.  Nemo
            // enim  ipsam  voluptatem,  quia
            // voluptas  sit,  aspernatur aut
            // odit   aut   fugit,  sed  quia
            // consequuntur   magni   dolores
            // eos,  qui  ratione  voluptatem
            // sequi  nesciunt,  neque  porro
            // quisquam   est,   qui  dolorem
            // ipsum,  quia  dolor sit, amet,
            // consectetur,  adipisci  velit,
            // sed quia non numquam eius modi
            // tempora incidunt, ut labore et
            // dolore  magnam aliquam quaerat
            // voluptatem.  Ut enim ad minima
            // veniam,      quis      nostrum
            // exercitationem  ullam corporis
            // suscipit  laboriosam,  nisi ut
            // aliquid    ex    ea    commodi
            // consequatur?  Quis  autem  vel
            // eum iure reprehenderit, qui in
            // ea  voluptate velit esse, quam
            // nihil  molestiae  consequatur,
            // vel  illum,  qui  dolorem  eum
            // fugiat,   quo  voluptas  nulla
            // pariatur?   At   vero  eos  et
            // accusamus    et   iusto   odio
            // dignissimos    ducimus,    qui
            // blanditiis         praesentium
            // voluptatum    deleniti   atque
            // corrupti, quos dolores et quas
            // molestias    excepturi   sint,
            // obcaecati    cupiditate    non
            // provident,  similique  sunt in
            // culpa,  qui  officia  deserunt
            // mollitia animi, id est laborum
            // et   dolorum  fuga.  Et  harum
            // quidem  rerum  facilis  est et
            // expedita    distinctio.    Nam
            // libero   tempore,  cum  soluta
            // nobis   est   eligendi  optio,
            // cumque   nihil   impedit,  quo
            // minus id, quod maxime placeat,
            // facere     possimus,     omnis
            // voluptas  assumenda est, omnis
            // dolor  repellendus. Temporibus
            // autem    quibusdam    et   aut
            // officiis   debitis  aut  rerum
            // necessitatibus  saepe eveniet,
            // ut  et  voluptates repudiandae
            // sint    et    molestiae    non
            // recusandae. Itaque earum rerum
            // hic    tenetur    a   sapiente
            // delectus,  ut  aut  reiciendis
            // voluptatibus   maiores   alias
            // consequatur   aut  perferendis
        }
        
        private string FixNewLines(string value)
        {
            return value?.Replace("\n", Environment.NewLine);
        }
    }
}