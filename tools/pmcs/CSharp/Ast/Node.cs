//
// Node.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;

namespace Xamarin.Pmcs.CSharp.Ast
{
	public class Role
	{
		public string Name { get; private set; }

		public Role (string name = null)
		{
			Name = name;
		}
	}

	public abstract class Node
	{
		public Role Role { get; private set; }
		public Node Parent { get; private set; }
		public Node NextSibling { get; private set; }
		public Node PreviousSibling { get; private set; }
		public Node FirstChild { get; private set; }
		public Node LastChild { get; private set; }

		public void AddChild (Node child, Role role = null)
		{
			if (role != null)
				child.Role = role;

			child.Parent = this;

			if (FirstChild == null) {
				LastChild = child;
				FirstChild = child;
			} else {
				LastChild.NextSibling = child;
				child.PreviousSibling = LastChild;
			}

			LastChild = child;
		}

		public void InsertChildBefore (Node nextSibling, Node child, Role role = null)
		{
			if (nextSibling == null) {
				AddChild (child, role ?? child.Role);
				return;
			}

			if (role != null)
				child.Role = role;

			child.Parent = this;
			child.NextSibling = nextSibling;
			child.PreviousSibling = nextSibling.PreviousSibling;

			if (nextSibling.PreviousSibling != null)
				nextSibling.PreviousSibling.NextSibling = child;
			else
				FirstChild = child;

			nextSibling.PreviousSibling = child;
		}

		public void Remove ()
		{
			if (Parent == null)
				return;

			if (PreviousSibling != null)
				PreviousSibling.NextSibling = NextSibling;
			else
				Parent.FirstChild = NextSibling;

			if (NextSibling != null)
				NextSibling.PreviousSibling = PreviousSibling;
			else
				Parent.LastChild = PreviousSibling;

			Parent = null;
			PreviousSibling = null;
			NextSibling = null;
		}

		public IEnumerable<Node> Children {
			get {
				Node next;
				for (var child = FirstChild; child != null; child = next) {
					next = child.NextSibling;
					yield return child;
				}
			}
		}

		public void SetChild (Node child, Role role)
		{
			if (child == null)
				throw new ArgumentNullException (nameof (child));

			if (role == null)
				throw new ArgumentNullException (nameof (role));

			child.Role = role;

			var existing = GetChild<Node> (role);
			if (existing == null) {
				AddChild (child, role);
				return;
			}

			existing.Parent.InsertChildBefore (existing, child);
			existing.Remove ();
		}

		public T GetChild<T> (Role role) where T : Node
		{
			return GetChildren<T> (role).FirstOrDefault ();
		}

		public IEnumerable<T> GetChildren<T> (Role role) where T : Node
		{
			return Children.Where (child => child.Role == role).Cast<T> ();
		}

		public abstract void AcceptVisitor (IAstVisitor visitor);

		public void Dump (int depth = 0)
		{
			Console.WriteLine ("{0}<{1}>: {2}", String.Empty.PadLeft (depth * 2), GetType (), this);
			for (var child = FirstChild; child != null; child = child.NextSibling)
				child.Dump (depth + 1);
		}
	}
}