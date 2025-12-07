namespace MG.Dynamic.Internal;

internal static class ParameterAttributeExtensions
{
	internal static void Reset(this ParameterAttribute attribute)
	{
		GetHelpMessageField(attribute) = null;
		GetHelpMessageBaseNameField(attribute) = null;
		GetHelpMessageResourceIdField(attribute) = null;
		GetEffectiveActionField(attribute) = default;
		GetExperimentNameField(attribute) = null;
		GetExperimentActionField(attribute) = default;

		attribute.DontShow = false;
		attribute.Mandatory = false;
		attribute.ParameterSetName = ParameterAttribute.AllParameterSets;
		attribute.Position = int.MinValue;
		attribute.ValueFromPipeline = false;
		attribute.ValueFromPipelineByPropertyName = false;
		attribute.ValueFromRemainingArguments = false;
	}

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_helpMessage")]
	private static extern ref string? GetHelpMessageField(ParameterAttribute attribute);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_helpMessageBaseName")]
	private static extern ref string? GetHelpMessageBaseNameField(ParameterAttribute attribute);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_helpMessageResourceId")]
	private static extern ref string? GetHelpMessageResourceIdField(ParameterAttribute attribute);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_effectiveAction")]
	private static extern ref ExperimentAction GetEffectiveActionField(ParameterAttribute attribute);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<ExperimentName>k__BackingField")]
	private static extern ref string? GetExperimentNameField(ParameterAttribute attribute);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<ExperimentAction>k__BackingField")]
	private static extern ref ExperimentAction GetExperimentActionField(ParameterAttribute attribute);
}
