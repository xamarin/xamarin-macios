using CoreGraphics;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PdfTagTypeTest {

		[Test]
		public void EnumExtension ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.That (CGPdfTagType.Document.GetName (), Is.EqualTo ("/Document"), "Document");
			Assert.That (CGPdfTagType.Part.GetName (), Is.EqualTo ("/Part"), "Part");
			Assert.That (CGPdfTagType.Art.GetName (), Is.EqualTo ("/Art"), "Art");
			Assert.That (CGPdfTagType.Section.GetName (), Is.EqualTo ("/Sect"), "Section");
			Assert.That (CGPdfTagType.Div.GetName (), Is.EqualTo ("/Div"), "Div");
			Assert.That (CGPdfTagType.BlockQuote.GetName (), Is.EqualTo ("/BlockQuote"), "BlockQuote");
			Assert.That (CGPdfTagType.Caption.GetName (), Is.EqualTo ("/Caption"), "Caption");
			Assert.That (CGPdfTagType.Toc.GetName (), Is.EqualTo ("/TOC"), "Toc");
			Assert.That (CGPdfTagType.Toci.GetName (), Is.EqualTo ("/TOCI"), "Toci");
			Assert.That (CGPdfTagType.Index.GetName (), Is.EqualTo ("/Index"), "Index");
			Assert.That (CGPdfTagType.NonStructure.GetName (), Is.EqualTo ("/NonStruct"), "NonStructure");
			Assert.That (CGPdfTagType.Private.GetName (), Is.EqualTo ("/Private"), "Private");

			Assert.That (CGPdfTagType.Paragraph.GetName (), Is.EqualTo ("/P"), "Paragraph");
			Assert.That (CGPdfTagType.Header.GetName (), Is.EqualTo ("/H"), "Header");
			Assert.That (CGPdfTagType.Header1.GetName (), Is.EqualTo ("/H1"), "Header1");
			Assert.That (CGPdfTagType.Header2.GetName (), Is.EqualTo ("/H2"), "Header2");
			Assert.That (CGPdfTagType.Header3.GetName (), Is.EqualTo ("/H3"), "Header3");
			Assert.That (CGPdfTagType.Header4.GetName (), Is.EqualTo ("/H4"), "Header4");
			Assert.That (CGPdfTagType.Header5.GetName (), Is.EqualTo ("/H5"), "Header5");
			Assert.That (CGPdfTagType.Header6.GetName (), Is.EqualTo ("/H6"), "Header6");

			Assert.That (CGPdfTagType.List.GetName (), Is.EqualTo ("/L"), "List");
			Assert.That (CGPdfTagType.ListItem.GetName (), Is.EqualTo ("/LI"), "ListItem");
			Assert.That (CGPdfTagType.Label.GetName (), Is.EqualTo ("/Lbl"), "Label");
			Assert.That (CGPdfTagType.ListBody.GetName (), Is.EqualTo ("/LBody"), "ListBody");

			Assert.That (CGPdfTagType.Table.GetName (), Is.EqualTo ("/Table"), "Table");
			Assert.That (CGPdfTagType.TableRow.GetName (), Is.EqualTo ("/TR"), "TableRow");
			Assert.That (CGPdfTagType.TableHeaderCell.GetName (), Is.EqualTo ("/TH"), "TableHeaderCell");
			Assert.That (CGPdfTagType.TableDataCell.GetName (), Is.EqualTo ("/TD"), "TableDataCell");
			Assert.That (CGPdfTagType.TableHeader.GetName (), Is.EqualTo ("/THead"), "TableHeader");
			Assert.That (CGPdfTagType.TableBody.GetName (), Is.EqualTo ("/TBody"), "TableBody");
			Assert.That (CGPdfTagType.TableFooter.GetName (), Is.EqualTo ("/TFoot"), "TableFooter");

			Assert.That (CGPdfTagType.Span.GetName (), Is.EqualTo ("/Span"), "Span");
			Assert.That (CGPdfTagType.Quote.GetName (), Is.EqualTo ("/Quote"), "Quote");
			Assert.That (CGPdfTagType.Note.GetName (), Is.EqualTo ("/Note"), "Note");
			Assert.That (CGPdfTagType.Reference.GetName (), Is.EqualTo ("/Reference"), "Reference");
			Assert.That (CGPdfTagType.Bibliography.GetName (), Is.EqualTo ("/BibEntry"), "Bibliography");
			Assert.That (CGPdfTagType.Code.GetName (), Is.EqualTo ("/Code"), "Code");
			Assert.That (CGPdfTagType.Link.GetName (), Is.EqualTo ("/Link"), "Link");
			Assert.That (CGPdfTagType.Annotation.GetName (), Is.EqualTo ("/Annot"), "Annotation");

			Assert.That (CGPdfTagType.Ruby.GetName (), Is.EqualTo ("/Ruby"), "Ruby");
			Assert.That (CGPdfTagType.RubyBaseText.GetName (), Is.EqualTo ("/RB"), "RubyBaseText");
			Assert.That (CGPdfTagType.RubyAnnotationText.GetName (), Is.EqualTo ("/RT"), "RubyAnnotationText");
			Assert.That (CGPdfTagType.RubyPunctuation.GetName (), Is.EqualTo ("/RP"), "RubyPunctuation");
			Assert.That (CGPdfTagType.Warichu.GetName (), Is.EqualTo ("/Warichu"), "Warichu");
			Assert.That (CGPdfTagType.WarichuText.GetName (), Is.EqualTo ("/WT"), "WarichuText");
			Assert.That (CGPdfTagType.WarichuPunctiation.GetName (), Is.EqualTo ("/WP"), "WarichuPunctiation");
			Assert.That (CGPdfTagType.Figure.GetName (), Is.EqualTo ("/Figure"), "Figure");
			Assert.That (CGPdfTagType.Formula.GetName (), Is.EqualTo ("/Formula"), "Formula");
			Assert.That (CGPdfTagType.Form.GetName (), Is.EqualTo ("/Form"), "Form");
		}
	}
}
