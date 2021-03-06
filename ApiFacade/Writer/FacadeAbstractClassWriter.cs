﻿using ApiFacade.Parser;

namespace ApiFacade.Writer
{
    public class FacadeAbstractClassWriter : FacadeClassWriter
    {
        public FacadeAbstractClassWriter(FacadeClass Class) : base(Class)
        {
        }

        protected override string ClassDeclaration => $"public abstract class {Class.Name} : {FacadeClass.ParentNamespaceName}.{Class.Name}";
    }
}
