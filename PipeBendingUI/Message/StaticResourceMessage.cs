namespace PipeBendingUI.Message;

public record ComponentChangedMessage(IMKernel.Model.Component? Value);

public record PropertiesUIFinishedMessage();
