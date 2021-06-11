using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Assets.Plugins.Alg;
using TowerGenerator.Components.DynamicGrowSegments;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator.ChunkImporter
{
    // see all commands: https://docs.google.com/spreadsheets/d/1sefKKZGdllpTpHPTX2AZMRrZCT5wT7QiN8GhPCoiGW4/edit#gid=0

    public static class FbxCommand
    {
        #region Fbx commands hierarchy
        private abstract class FbxCommandBase
        {
            public class Parameter<T>
            {
                public string Name;
                public T Value;
            }

            public abstract string GetFbxCommandName();
            public abstract string GetPayloadCommandName();
            public abstract void ParseParameters(string parameters, GameObject gameObject);
            public abstract void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation);

            protected static float ConvertFloat01(string float01String)
            {
                float value = float.Parse(float01String);

                var clamped = Mathf.Clamp(value, 0.0f, 1.0f);
                if (Math.Abs(clamped - value) > 0.00001f)
                    Debug.LogWarning($"Clamping happened {value} -> {clamped}");
                return clamped;
            }

            protected static int ConvertInt(string intString)
            {
                int value = Int32.Parse(intString);
                return value;
            }

            protected static bool ConvertBool(string boolString)
            {
                bool value = bool.Parse(boolString);
                return value;
            }

            public virtual int GetExecutionPriority()
            {
                return int.MaxValue;
            }

            protected T ConvertEnum<T>(string enumString) where T : struct
            {
                try
                {
                    enumString = enumString.Replace('|', ',');
                    T res = (T)Enum.Parse(typeof(T), enumString);
                    if (!Enum.IsDefined(typeof(T), res))
                        return default(T);
                    return res;
                }
                catch
                {
                    return default(T);
                }
            }
        }

        private abstract class FbxCommandAddComponent : FbxCommandBase
        {
            public override string GetFbxCommandName()
            {
                return "AddComponent";
            }

            public override int GetExecutionPriority()
            {
                return 0;
            }
        }

        private abstract class FbxCommandAddAttribute : FbxCommandBase
        {
            public override string GetFbxCommandName()
            {
                return "AddAttribute";
            }

            public override int GetExecutionPriority()
            {
                return 1; // attributes has lower priority (0 is higher priority than 1) because usually it used for configure already assigned components which has to be added by AddComponent with higher priority
            }
        }
        #endregion

        #region Groups
        private class GroupStack : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "GroupStack";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<global::TowerGenerator.GroupStack>();
                importInformation.GroupStackAmount++;
            }
        }

        private class GroupSet : FbxCommandAddComponent
        {
            private Parameter<int> _minObjectsSelected;
            private Parameter<int> _maxObjectsSelected;

            public override string GetPayloadCommandName()
            {
                return "GroupSet";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _minObjectsSelected = new Parameter<int> { Name = "MinObjectsSelected", Value = 0 };
                _maxObjectsSelected = new Parameter<int> { Name = "MaxObjectsSelected", Value = gameObject.transform.childCount };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                var actualParams = parameters.Split(',');
                Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);
                if (actualParams.Length >= 1)
                {
                    _minObjectsSelected.Value = ConvertInt(actualParams[0]);
                }
                if (actualParams.Length >= 2)
                {
                    _maxObjectsSelected.Value = ConvertInt(actualParams[1]);
                }
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var groupSetComp = gameObject.AddComponent<global::TowerGenerator.GroupSet>();
                groupSetComp.MaxObjectsSelected = _maxObjectsSelected.Value;
                groupSetComp.MinObjectsSelected = _minObjectsSelected.Value;
                importInformation.GroupSetAmount++;
            }
        }

        private class GroupSwitch : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "GroupSwitch";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<global::TowerGenerator.GroupSwitch>();
                importInformation.GroupSwitchAmount++;
            }
        }
        #endregion

        private class ChunkController : FbxCommandAddComponent
        {
            private Parameter<TopologyType> _chunkTopologyType;
            private Parameter<ChunkConformationType> _chunkConformationType;

            public override string GetPayloadCommandName()
            {
                return "ChunkController";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _chunkTopologyType = new Parameter<TopologyType> { Name = "ChunkTopologyType", Value = TopologyType.ChunkStd };
                _chunkConformationType = new Parameter<ChunkConformationType> { Name = "ChunkConformationTypes", Value = ChunkConformationType.DimensionsBased };

                var actualParams = parameters.Split(',');
                Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);
                if (actualParams.Length >= 1)
                {
                    _chunkTopologyType.Value = ConvertEnum<TopologyType>(actualParams[0]);
                    Assert.IsTrue(_chunkTopologyType.Value != TopologyType.Undefined, $"Can't parse 'ChunkTopologyType' parameter from value '{actualParams[0]}'");
                }
                if (actualParams.Length >= 2)
                {
                    _chunkConformationType.Value = ConvertEnum<ChunkConformationType>(actualParams[1]);
                    Assert.IsTrue(_chunkConformationType.Value != ChunkConformationType.Undefined, $"Can't parse 'ChunkConformationType' parameter from value '{actualParams[1]}'");
                }
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                ChunkControllerBase chunkController = null;

                if (_chunkConformationType.Value == ChunkConformationType.Combinatorial)
                    chunkController = gameObject.AddComponent<ChunkControllerCombinatorial>();
                else if (_chunkConformationType.Value == ChunkConformationType.DimensionsBased)
                    chunkController = gameObject.AddComponent<ChunkControllerDimensionsBased>();
                else if (_chunkConformationType.Value == ChunkConformationType.DynamicGrow)
                    chunkController = gameObject.AddComponent<ChunkControllerDynamicGrow>();
                else if (_chunkConformationType.Value == ChunkConformationType.Stretchable)
                    chunkController = gameObject.AddComponent<ChunkControllerStretchable>();

                Assert.IsNotNull(chunkController, "chunk controller is null");
                chunkController.TopologyType = _chunkTopologyType.Value;
                chunkController.ConformationType = _chunkConformationType.Value;
                importInformation.ChunkControllerAmount++;
                if(!importInformation.ConformationType.ContainsKey(_chunkConformationType.Value))
                    importInformation.ConformationType.Add(_chunkConformationType.Value, 0);
                importInformation.ConformationType[_chunkConformationType.Value]++;
            }
        }

        #region Node attributes
        private class CollisionDependent : FbxCommandAddAttribute
        {
            private Parameter<global::TowerGenerator.CollisionDependant.CollisionCheckMode> _collisionMode;
            public override string GetPayloadCommandName()
            {
                return "CollisionDependent";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set default values first
                _collisionMode = new Parameter<global::TowerGenerator.CollisionDependant.CollisionCheckMode> { Name = "CollisionMode", Value = global::TowerGenerator.CollisionDependant.CollisionCheckMode.MeshBased };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                parameters = parameters.Trim();

                Assert.IsTrue(parameters.All(char.IsLetter));

                _collisionMode.Value = ConvertEnum<global::TowerGenerator.CollisionDependant.CollisionCheckMode>(parameters);
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.CollisionDependant>();
                comp.CollisionCheck = _collisionMode.Value;
                importInformation.CollisionDependentAmount++;
            }
        }

        private class DimensionsIgnorant : FbxCommandAddAttribute
        {
            public override string GetPayloadCommandName()
            {
                return "DimensionsIgnorant";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<global::TowerGenerator.DimensionsIgnorant>();
                importInformation.DimensionsIgnorantAmount++;
            }
        }

        private class DimensionsStack : FbxCommandAddAttribute
        {
            public override string GetPayloadCommandName()
            {
                return "DimensionsStack";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var dimBasedController = gameObject.GetComponentInParent<ChunkControllerDimensionsBased>();
                Assert.IsNotNull(dimBasedController, "DimensionsStack attribute refers to GroupStack group that controls dim based logic, so  ChunkControllerDimensionsBased must be presented");

                var groupStack = gameObject.GetComponent<global::TowerGenerator.GroupStack>();
                Assert.IsNotNull(groupStack, "Have to be attached because of higher priority of AddComponent");

                dimBasedController.DimensionStack = groupStack;
                importInformation.DimensionsStackAmount++;
            }
        }

        private class Suppression : FbxCommandAddAttribute
        {
            private string[] _suppressionLabels;

            public override string GetPayloadCommandName()
            {
                return "Suppression";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _suppressionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.Suppression>();
                comp.SuppressionLabels = _suppressionLabels;
                importInformation.SuppressionAmount++;
            }
        }

        private class SuppressedBy : FbxCommandAddAttribute
        {
            private string[] _suppressionLabels;
            public override string GetPayloadCommandName()
            {
                return "SuppressedBy";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _suppressionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.SuppressedBy>();
                comp.SuppressionLabels = _suppressionLabels;
                importInformation.SuppressedByAmount++;
            }
        }

        private class Induction : FbxCommandAddAttribute
        {
            private string[] _inductionLabels;
            public override string GetPayloadCommandName()
            {
                return "Induction";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _inductionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.Induction>();
                comp.InductionLabels = _inductionLabels;
                importInformation.InductionAmount++;
            }
        }

        private class InducedBy : FbxCommandAddAttribute
        {
            private string[] _inductionLabels;
            public override string GetPayloadCommandName()
            {
                return "InducedBy";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _inductionLabels = parameters.Split(',');
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var comp = gameObject.AddComponent<global::TowerGenerator.InducedBy>();
                comp.InductionLabels = _inductionLabels;
                importInformation.InducedByAmount++;
            }
        }

        private class Hidden : FbxCommandAddAttribute
        {
            public override string GetPayloadCommandName()
            {
                return "Hidden";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<global::TowerGenerator.Hidden>();
                importInformation.HiddenAmount++;
            }
        }

        private class ClassName : FbxCommandAddAttribute
        {
            private string[] _classNames;
            public override string GetPayloadCommandName()
            {
                return "ClassName";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));
                _classNames = parameters.Split('|');
                _classNames = _classNames.Select(RemoveSpaces).ToArray();
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                importInformation.ClassNameAmount++;
                importInformation.ChunkClass = _classNames;
            }
        }

        private class Generation : FbxCommandAddAttribute
        {
            private Parameter<uint> _generation;
            public override string GetPayloadCommandName()
            {
                return "Generation";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _generation = new Parameter<uint> { Name = "GenerationValue", Value = 0 };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                _generation.Value = (uint)ConvertInt(parameters);
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                importInformation.GenerationAttributeAmount++;
                importInformation.Generation = _generation.Value;
            }
        }

        private class ShapeConfiguration : FbxCommandAddAttribute
        {
            private Parameter<ChunkShapeConfigurationType> _shapeConfigurationTypeParameter;

            public override string GetPayloadCommandName()
            {
                return "ShapeConfiguration";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                _shapeConfigurationTypeParameter = new Parameter<ChunkShapeConfigurationType> { Name = "ShapeConfiguration", Value = ChunkShapeConfigurationType.Unspecified};
                if (parameters.Length >= 1)
                    _shapeConfigurationTypeParameter.Value = ConvertEnum<ChunkShapeConfigurationType>(parameters);
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var controllerBase = gameObject.GetComponent<ChunkControllerBase>();
                Assert.IsNotNull(controllerBase);
                controllerBase.ShapeConfigurationType = _shapeConfigurationTypeParameter.Value;
            }
        }

        private class DynamicGrowSegmentAttribute : FbxCommandAddAttribute
        {
            private Parameter<DynamicGrowSegmentType> _dynamicGrowSegmentParameter;

            public override string GetPayloadCommandName()
            {
                return "DynamicGrowSegment";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                _dynamicGrowSegmentParameter = new Parameter<DynamicGrowSegmentType> { Name = "DynamicGrowSegment", Value = DynamicGrowSegmentType.MiddleSegment }; // set default value
                if (parameters.Length >= 1)
                    _dynamicGrowSegmentParameter.Value = ConvertEnum<DynamicGrowSegmentType>(parameters);
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<DynamicGrowSegment>().SegmentType = _dynamicGrowSegmentParameter.Value;
            }
        }
        #endregion

        #region Miscellaneous logic

        private class Connector : FbxCommandAddComponent
        {
            public override string GetPayloadCommandName()
            {
                return "Connector";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                gameObject.AddComponent<global::TowerGenerator.Connector>();
                importInformation.ConnectorAmount++;
            }
        }

        private class Tag : FbxCommandAddAttribute
        {
            private Parameter<string> _tagName;
            private Parameter<float> _tagValue;

            public override string GetPayloadCommandName()
            {
                return "Tag";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _tagName = new Parameter<string> { Name = "TagName", Value = "" };
                _tagValue = new Parameter<float> { Name = "TagValue", Value = 1f };

                Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));

                var actualParams = parameters.Split(',');
                Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);

                if (actualParams.Length >= 1)
                {
                    _tagName.Value = actualParams[0];
                }

                if (actualParams.Length >= 2)
                {
                    _tagValue.Value = ConvertFloat01(actualParams[1]);
                }
            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var tagHolder = gameObject.GetComponent<TagHolder>();
                if (tagHolder == null)
                    tagHolder = gameObject.AddComponent<TagHolder>();

                tagHolder.TagSet = new TagSet();
                tagHolder.TagSet.SetTag(_tagName.Value, _tagValue.Value);
                
                importInformation.TagAmount++;
            }
        }

        private class ComponentValue : FbxCommandAddAttribute
        {
            private Parameter<string> _componentName;
            private Parameter<string> _componentPropertyString;
            public override string GetPayloadCommandName()
            {
                return "ComponentValue";
            }

            public override void ParseParameters(string parameters, GameObject gameObject)
            {
                // set defaults parameters first
                _componentName = new Parameter<string> { Name = "UnityComponentName", Value = "" };
                _componentPropertyString = new Parameter<string>() { Name = "ComponentPropertyString", Value = "" };

                if (string.IsNullOrWhiteSpace(parameters))
                    return;

                var actualParams = parameters.Split(new char[] { ',' }, 2);
                _componentName.Value = actualParams[0].Trim();
                if (actualParams.Length > 1)
                    _componentPropertyString.Value = actualParams[1].Trim();
                else
                    _componentPropertyString = null;

            }

            public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
            {
                var splitedComponentStrings = _componentName.Value.Split(new char[] { '[', ']' });
                Assert.IsTrue(splitedComponentStrings.Length == 3);

                var componentName = splitedComponentStrings[0];
                var componentIndex = Convert.ToInt32(splitedComponentStrings[1]);
                Type compType = GetType(componentName);

                var components = gameObject.GetComponents(compType);
                if (components == null || components.Length <= componentIndex)
                {
                    gameObject.AddComponent(compType);
                    components = gameObject.GetComponents(compType);
                }

                var comp = components[componentIndex];

                if (_componentPropertyString != null)
                {
                    var valueStrings = _componentPropertyString.Value.Split('=');
                    var parameterName = valueStrings[0];
                    var parameterValue = valueStrings[1];
                    var fieldInfo = compType.GetField(parameterName,
                        System.Reflection.BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    var fType = fieldInfo.FieldType;


                    object fieldValue;
                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                        fieldValue = typeConverter.ConvertFromInvariantString(parameterValue);
                    }
                    catch (Exception)
                    {
                        fieldValue = _implicitConvert(parameterValue, fType);
                    }


                    fieldInfo.SetValue(comp, fieldValue);
                }
            }

            // custom types convertors

            object _implicitConvert(string valueString, Type type)
            {
                if (type == typeof(Vector3))
                    return StringToVector3(valueString);
                throw new NotImplementedException($"{valueString} has no converter");
            }

            object StringToVector3(string v3String)
            {
                // Remove the parentheses
                if (v3String.StartsWith("(") && v3String.EndsWith(")"))
                {
                    v3String = v3String.Substring(1, v3String.Length - 2);
                }

                // split the items
                string[] sArray = v3String.Split(',');

                // store as a Vector3
                Vector3 result = new Vector3(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2]));

                return result;
            }



            public static Type GetType(string TypeName)
            {

                // Try Type.GetType() first. This will work with types defined
                // by the Mono runtime, in the same assembly as the caller, etc.
                var type = Type.GetType(TypeName);

                // If it worked, then we're done here
                if (type != null)
                    return type;

                // If the TypeName is a full name, then we can try loading the defining assembly directly
                if (TypeName.Contains("."))
                {

                    // Get the name of the assembly (Assumption is that we are using 
                    // fully-qualified type names)
                    var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                    // Attempt to load the indicated Assembly
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly == null)
                        return null;

                    // Ask that assembly to return the proper Type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;

                }

                // If we still haven't found the proper type, we can enumerate all of the 
                // loaded assemblies and see if any of them define the type
                var currentAssembly = Assembly.GetExecutingAssembly();
                var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
                foreach (var assemblyName in referencedAssemblies)
                {

                    // Load the referenced assembly
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        // See if that assembly defines the named type
                        type = assembly.GetType(TypeName);
                        if (type != null)
                            return type;
                    }
                }

                // The type just couldn't be found...
                return null;

            }
        }
        #endregion



        private static FbxCommandBase[] _fbxCommands =
        {
            // Groups
            new GroupStack(),
            new GroupSet(),
            new GroupSwitch(),

            new ChunkController(),
            new Connector(),
            new Tag(),

            // Node attributes
            new CollisionDependent(),
            new DimensionsIgnorant(),
            new DimensionsStack(),
            new Suppression(),
            new SuppressedBy(),
            new Induction(),
            new InducedBy(),
            new Hidden(),
            new ClassName(), 
            new Generation(),
            new ShapeConfiguration(), 
            new DynamicGrowSegmentAttribute(), 
            new ComponentValue(), 
        };

        public static void Execute(FbxProps fromFbxProps, GameObject gameObject, ChunkCooker.ChunkImportInformation chunkImportInformation)
        {
            Assert.IsNotNull(fromFbxProps);
            Assert.IsNotNull(gameObject);
            Assert.IsNotNull(fromFbxProps.Properties, $"empty props on {gameObject}");

            var commands = new List<FbxCommandBase>(fromFbxProps.Properties.Count);

            // parse commands
            foreach (var property in fromFbxProps.Properties)
            {
                string fbxCmdName = ParseFbxCommand(property);
                string payloadCmd = ParsePayloadCommand(property);
                string payloadParameters = ParsePayloadParameters(property);

                var cmd = _fbxCommands.FirstOrDefault(x => x.GetFbxCommandName() == fbxCmdName && x.GetPayloadCommandName() == payloadCmd);
                if (cmd == null)
                    Debug.LogError($"Unable to find cmd '{fbxCmdName}', payloadCmd = '{payloadCmd}', payloadParameters = '{payloadParameters}', object = '{gameObject.transform.GetDebugName()}' ");
                cmd.ParseParameters(payloadParameters, gameObject);
                commands.Add(cmd);
                chunkImportInformation.CommandsProcessedAmount++;
            }

            // execute commands by their priorities
            foreach (var cmd in commands.OrderBy(c => c.GetExecutionPriority()))
                cmd.Execute(gameObject, chunkImportInformation);
        }

        private static string RemoveSpaces( string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }

        private static string ParsePayloadCommand(FbxProps.Property property)
        {
            var match = Regex.Match(property.Value, @"^[^\( ]+");
            return match.Groups[0].Value;
        }

        // info: fbx command could end with digit due to fbx props naming traits
        private static string ParseFbxCommand(FbxProps.Property property)
        {
            return Regex.Replace(property.Name, @"\d*$", "");
        }

        // get string inside parentheses 
        private static string ParsePayloadParameters(FbxProps.Property property)
        {
            var matchGroups = Regex.Match(property.Value, @"\(([^\)]+)\)").Groups;
            return matchGroups[1].Value;
        }
    }
}
