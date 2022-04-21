namespace DistIL.IR;

//Maybe IntrinsicId.ArrayLen?
public class ArrayLenInst : Instruction
{
    public Value Array {
        get => Operands[0];
        set => ReplaceOperand(0, value);
    }
    public override string InstName => "arrlen";
    public override bool MayThrow => true;

    public ArrayLenInst(Value array)
        : base(array)
    {
        ResultType = PrimType.IntPtr;
    }

    public override void Accept(InstVisitor visitor) => visitor.Visit(this);
}

public abstract class ArrayAccessInst : Instruction
{
    public override bool MayThrow => true;

    public Value Array {
        get => Operands[0];
        set => ReplaceOperand(0, value);
    }
    public Value Index {
        get => Operands[1];
        set => ReplaceOperand(1, value);
    }
    public ArrayAccessFlags Flags { get; set; }

    protected ArrayAccessInst(ArrayAccessFlags flags, params Value[] operands)
        : base(operands)
    {
        Flags = flags;
    }
}

public class LoadArrayInst : ArrayAccessInst
{
    public RType ElemType {
        get => ResultType;
        set => ResultType = value;
    }
    public override string InstName => "ldarr";

    public LoadArrayInst(Value array, Value index, RType elemType, ArrayAccessFlags flags = 0)
        : base(flags, array, index)
    {
        ElemType = elemType;
    }

    public override void Accept(InstVisitor visitor) => visitor.Visit(this);
}
public class StoreArrayInst : ArrayAccessInst
{
    public Value Value {
        get => Operands[2];
        set => ReplaceOperand(2, value);
    }
    public RType ElemType { get; set; }

    public override string InstName => "starr";
    public override bool HasSideEffects => true;

    public StoreArrayInst(Value array, Value index, Value value, RType elemType, ArrayAccessFlags flags = 0)
        : base(flags, array, index, value)
    {
        ElemType = elemType;
    }

    public override void Accept(InstVisitor visitor) => visitor.Visit(this);
}
public class ArrayAddrInst : ArrayAccessInst
{
    public override string InstName => "arraddr";

    public ArrayAddrInst(Value array, Value index, ArrayAccessFlags flags = 0)
        : base(flags, array, index)
    {
        ResultType = new ByrefType(((ArrayType)array.ResultType).ElemType);
    }

    public override void Accept(InstVisitor visitor) => visitor.Visit(this);
}

public enum ArrayAccessFlags
{
    None            = 0,
    NoBoundsCheck   = 1 << 0,
    NoTypeCheck     = 1 << 1,
    NoNullCheck     = 1 << 2,
    ReadOnly        = 1 << 4,
}