// Test for custom attributes on a 'undefined' type parameter index

.assembly extern mscorlib
{
}
.assembly 'err-gen-cattr-3'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module 'err-gen-cattr-3.exe'

.class private auto ansi beforefieldinit GenParAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 00 40 00 00 00 00 )
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  }

}

.class private auto ansi beforefieldinit cons`1<A>
       extends [mscorlib]System.Object
{
  /* Type parameter index is 1-based */	  
  .param type [2]
  .custom instance void GenParAttribute::.ctor() = ( 01 00 00 00 ) 

 .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  }

}

