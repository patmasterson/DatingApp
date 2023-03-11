using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepo, IMessageRepository messageRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username.ToLower() == createMessageDto.RecipientUserName.ToLower())
                return BadRequest("You cannot send a message to yourself");

            var sender = await _userRepo.GetUserByUserNameAsync(username);
            var recipient = await _userRepo.GetUserByUserNameAsync(createMessageDto.RecipientUserName); 

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepo.AddMessage(message);

            if (await _messageRepo.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");


        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();
            var messages = await _messageRepo.GetMessagesForUser(messageParams);
            var pageHeader = new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            Response.AddPaginationHeader(pageHeader);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return Ok(await _messageRepo.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id) 
        {
            var userName = User.GetUserName();
            var message = await _messageRepo.GetMessage(id);
            if (message.SenderUserName != userName && message.RecipientUserName != userName) 
                return Unauthorized();

            if (message.SenderUserName.ToLower() == userName.ToLower()) message.SenderDeleted = true;
            if (message.RecipientUserName.ToLower() == userName.ToLower()) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepo.DeleteMessage(message);
                
            }

            if (await _messageRepo.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting message");
        }
    }
}