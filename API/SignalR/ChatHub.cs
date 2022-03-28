using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Application.Comments;

namespace API.SignalR
{
  public class ChatHub : Hub  // inherit from AspNetCore.SignalR.Hub
  {
    private readonly IMediator _mediator;
    public ChatHub(IMediator mediator)
    {
      _mediator = mediator;
    }

    // send the comment to whom?
    // the 'SendComment' must match with the invoke method in commentStore - addComment
    public async Task SendComment(Create.Command command)
    {
        var comment = await _mediator.Send(command);

        // send the comment to whom ? all users or the comment creator or 
        // the one participated in the activity like in this case
        await Clients.Group(command.ActivityId.ToString())  
            .SendAsync("ReceiveComment", comment.Value);  // 'ReceiveComment' must match with the same method on commentStore - hubConnection.on('ReceiveComment')
    }

    // when a client connect to an activity, add them to the group (the group used in SendComment above)
    // and send them the list of activity
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var activityId = httpContext.Request.Query["activityId"];
        await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
        var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)});
        await Clients.Caller.SendAsync("LoadComments", result.Value);  // LoadComments text must match with same text in client side commentStore
    }
  }
}