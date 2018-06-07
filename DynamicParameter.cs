using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

namespace Dynamic
{
    public class DynamicLibrary : Collection<DynamicParameter>
    {
        public DynamicLibrary()
        {
        }

        public DynamicLibrary(DynamicParameter dynParam)
        {
            this.Add(dynParam);
        }

        public DynamicLibrary(string paramName, string[] validatedItems)
        {
            DynamicParameter dynParam = new DynamicParameter(paramName, validatedItems);
            this.Add(dynParam);
        }
        public DynamicLibrary(string paramName, string[] validatedItems, Hashtable attributes)
        {
            DynamicParameter dynParam = new DynamicParameter(paramName, validatedItems, attributes);
            this.Add(dynParam);
        }
        public DynamicLibrary(string paramName, string[] validatedItems, Hashtable attributes, string[] aliases)
        {
            DynamicParameter dynParam = new DynamicParameter(paramName, validatedItems, attributes, aliases);
            this.Add(dynParam);
        }
        public DynamicLibrary(string paramName, string[] validatedItems, Hashtable attributes, string[] aliases, Type runtimeType, bool allowNull = false)
        {
            DynamicParameter dynParam = new DynamicParameter(paramName, validatedItems, attributes, aliases, runtimeType, allowNull);
            this.Add(dynParam);
        }
        
        // ADD NEW DYNAMIC PARAMETER
        public void AddNewParameter(string paramName, string[] validatedItems, Type runtimeType, Hashtable attributes = null, string[] aliases = null, bool allowNull = false)
        {
            DynamicParameter dynParam = new DynamicParameter(paramName, validatedItems, attributes, aliases, runtimeType, allowNull);
            this.Add(dynParam);
        }

        // COMBINE PARAMETERS
        public RuntimeDefinedParameterDictionary Generate()
        {
            RuntimeDefinedParameterDictionary rtDict = new RuntimeDefinedParameterDictionary();
            foreach (DynamicParameter dynParam in this)
            {
                RuntimeDefinedParameter rtParam = dynParam.Create();
                rtDict.Add(dynParam.Name, rtParam);
            }
            return rtDict;
        }
    }

    public class DynamicParameter
    {
        public string Name { get; set; }
        public Type RuntimeType = Type.GetType("System.String");
        public string[] ValidatedItems { get; set; }
        public Hashtable Attributes { get; set; }
        public string[] Aliases { get; set; }
        public bool AllowNull = false;

        public DynamicParameter() { }
        public DynamicParameter(string paramName)
        {
            Name = paramName;
        }
        public DynamicParameter(string paramName, string[] validatedItems)
        {
            string[] array = new string[validatedItems.Length];
            Name = paramName;
            ValidatedItems = validatedItems;
        }
        public DynamicParameter(string paramName, string[] validatedItems, Hashtable attributes)
        {
            Name = paramName;
            ValidatedItems = validatedItems;
            Attributes = attributes;
        }
        public DynamicParameter(string paramName, string[] validatedItems, Hashtable attributes, string[] aliases)
        {
            Name = paramName;
            ValidatedItems = validatedItems;
            Attributes = attributes;
            Aliases = aliases;
        }
        public DynamicParameter(string paramName, string[] validatedItems, Hashtable attributes, string[] aliases, Type runtimeType, bool allowNull = false)
        {
            Name = paramName;
            ValidatedItems = validatedItems;
            RuntimeType = runtimeType;
            Attributes = attributes;
            Aliases = aliases;
            AllowNull = allowNull;
        }

        public static T Cast<T>(object o)
        {
            return (T)o;
        }

        public RuntimeDefinedParameter Create()
        {
            Collection<Attribute> attCol = new Collection<Attribute>();
            if (Attributes != null)
            {
                ParameterAttribute pAtt = new ParameterAttribute();
                foreach (PropertyInfo info in pAtt.GetType().GetProperties())
                {
                    foreach (string key in Attributes.Keys)
                    {
                        if (info.Name == key)
                        {
                            MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(info.PropertyType);
                            object castedObject = castMethod.Invoke(null, new object[] { Attributes[key] });
                            info.SetValue(pAtt, castedObject, null);
                        }
                    }
                }
                attCol.Add(pAtt);
            }

            ValidateSetAttribute valSet = new ValidateSetAttribute((string[])ValidatedItems);
            attCol.Add(valSet);

            if (Aliases != null)
            {
                AliasAttribute aliases = new AliasAttribute((string[])Aliases);
                attCol.Add(aliases);
            }

            if (AllowNull)
            {
                attCol.Add(new AllowNullAttribute());
            }

            RuntimeDefinedParameter rtParam = new RuntimeDefinedParameter(Name, RuntimeType, attCol);
            return rtParam;
        }

        public RuntimeDefinedParameterDictionary GenerateLibrary()
        {
            RuntimeDefinedParameter rtParam = Create();
            RuntimeDefinedParameterDictionary rtDict = new RuntimeDefinedParameterDictionary();
            rtDict.Add(this.Name, rtParam);
            return rtDict;
        }
    }
}