using IceSync.Services.Models;

namespace IceSync.AppClient;

public interface IAppHttpClient<InputT, OutputT>
{
    Task<OutputTypeModel<OutputT>> Execute(InputT inputT, Func<InputTypeModel<InputT>, Task<OutputTypeModel<OutputT>>> method, byte counter = 0);
}