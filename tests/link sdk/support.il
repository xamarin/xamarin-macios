// ilasm -dll support.il

.assembly support { }

.class public support {
	.method static public void throw_object () {
		newobj instance void class [mscorlib]System.Object::'.ctor'()
		throw
		ret
	}
}
