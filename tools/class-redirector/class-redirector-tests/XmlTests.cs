using ClassRedirector;
using System.Xml.Linq;

namespace ClassRedirectorTests;

[TestFixture]
public class XmlTests {
	[Test]
	public void WritesCorrectXml ()
	{
		var map = new CSToObjCMap () {
			["Foo"] = new ObjCNameIndex ("Bar", 2),
			["Baz"] = new ObjCNameIndex ("Zed", 3),
		};

		var doc = CSToObjCMap.ToXDocument (map);

		var elem = doc.Elements ().Where (el => el.Name == "CSToObjCMap").FirstOrDefault ();
		Assert.IsNotNull (elem, "no map");
		Assert.That (elem.Elements ().Count (), Is.EqualTo (2), "Incorrect number of children");

		var cselem = XElementWithAttribute (elem, "CSName", "Foo");
		Assert.IsNotNull (cselem, "missing Foo elem");
		var nameElem = NameElem (cselem);
		Assert.IsNotNull (nameElem, "no name elem1");
		Assert.That (nameElem.Value, Is.EqualTo ("Bar"));
		var indexElem = IndexElem (cselem);
		Assert.IsNotNull (indexElem, "no value elem1");
		Assert.That ((int) indexElem, Is.EqualTo (2));

		cselem = XElementWithAttribute (elem, "CSName", "Baz");
		Assert.IsNotNull (cselem, "missing Baz elem");
		nameElem = NameElem (cselem);
		Assert.IsNotNull (nameElem, "no name elem2");
		indexElem = IndexElem (cselem);
		Assert.That (nameElem.Value, Is.EqualTo ("Zed"));
		Assert.IsNotNull (indexElem, "no value elem2");
		Assert.That ((int) indexElem, Is.EqualTo (3));
	}

	[Test]
	public void ReadsCorrectXml ()
	{
		var text = @"<?xml version=""1.0"" encoding=""utf-8""?>
<CSToObjCMap>
  <Element CSName=""Foo"">
    <ObjNameIndex>
      <Name>Bar</Name>
      <Index>2</Index>
    </ObjNameIndex>
  </Element>
  <Element CSName=""Baz"">
    <ObjNameIndex>
      <Name>Zed</Name>
      <Index>3</Index>
    </ObjNameIndex>
  </Element>
</CSToObjCMap>";

		using var reader = new StringReader (text);
		var doc = XDocument.Load (reader);

		var map = CSToObjCMap.FromXDocument (doc);
		Assert.IsNotNull (map, "no map");
		Assert.That (map.Count (), Is.EqualTo (2));

		Assert.True (map.TryGetValue ("Foo", out var nameIndex), "no nameIndex");
		Assert.That (nameIndex.ObjCName, Is.EqualTo ("Bar"), "no bar name");
		Assert.That (nameIndex.MapIndex, Is.EqualTo (2), "no bar index");

		Assert.True (map.TryGetValue ("Baz", out var nameIndex1));
		Assert.That (nameIndex1.ObjCName, Is.EqualTo ("Zed"), "no bar name");
		Assert.That (nameIndex1.MapIndex, Is.EqualTo (3), "no bar index");
	}


	static XElement? XElementWithAttribute (XElement el, string attrName, string attrValue)
	{
		return el.Elements ().Where (e => {
			var attr = e.Attribute (attrName);
			return attr is not null && attr.Value == attrValue;
		}).FirstOrDefault ();
	}

	static XElement? NameElem (XElement el) => FirstElementNamed (el, "Name");

	static XElement? IndexElem (XElement el) => FirstElementNamed (el, "Index");

	static XElement? FirstElementNamed (XElement el, string name)
	{
		return el.Descendants ().Where (e => e.Name == name).FirstOrDefault ();
	}
}

