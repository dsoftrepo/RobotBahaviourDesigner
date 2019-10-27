using System.Threading.Tasks;

namespace RobotBehaviourDesigner.UI.View.Services
{
	public interface IMessageDialogService
	{
		Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
		Task ShowInfoDialogAsync(string text);
	}
}