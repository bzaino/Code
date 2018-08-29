define([
	'jquery',
	'underscore',
	'configuration',
	'salt',
	'salt/models/SiteMember'
], function ($, _, Configuration, SALT, SiteMember) {
	
	SiteMember.done(function (siteMember) {
		var PROJECT_ID = Configuration.chatbotAgents.SLR.projectId;

		var SESSION_ID = siteMember.IndividualId;
		var activeQuickReplyQuestion;

		var stringTranscript = '';
		//We need to change the content of the global lhnCustom variables to pass across the transcript
		//They give us 3 variables are already used elsewhere
		//For a chatbot handoff, we will move the third variable ("organizationName") into the first variable alongside the name
		//Then we put the transcript in the 3rd custom variable
		var usernameLivechat = lhnCustom1;
		var orgLiveChat = lhnCustom3;

		//Store the contexts since dialogflow only stores them for 5 minutes on their end before expiring.
		//Allows the user to go away from the chat for as long as they like before resuming
		//https://discuss.api.ai/t/how-to-increase-context-life-span/1008/5
		var currentContext = [];

		function renderBotResponse(response) {
			var messages = response.queryResult.fulfillmentMessages;
			var hasFancy = hasFancyFormat(messages);
			var timeout = 500;
			var currIncr = 0;
			$('#insert-msg-before').addClass('active');
			_.each(messages, function (msg, ind) {
				if (msg.platform === 'FACEBOOK' || (msg.platform === 'PLATFORM_UNSPECIFIED' && !hasFancy)) {
					setTimeout(function () {
						switch (msg.message) {
						case 'text':
							stringTranscript += 'Hope: ' + msg.text.text[0] + '\n';
							textResponse(msg.text.text[0]);
							break;
						case 'quickReplies':
							stringTranscript += 'Hope: ' + msg.quickReplies.title + ' ' + msg.quickReplies.quickReplies.join('|') + '\n';
							quickReplyResponse(msg.quickReplies.title, msg.quickReplies.quickReplies);
							break;
						case 'payload':
							customPayload(msg.payload);
							break; 
						default:
							break;
						}
					}, timeout);
					// we want to increment the timer for the next message based on the estimated 
					// time it takes to read this one
					if (msg.message !== 'text') {
						// doesn't really matter, there likely won't be any messages in the group after a 
						// quick-choose message
						currIncr = 2500;
					} else {
						// timeout between 50 and 100 times the number of character in the speech.
						var numWords = msg.text.text[0].split(' ').length;
						currIncr = Math.min(numWords * 300, 5000);
					}
					timeout += currIncr;
					
				}
			});
			setTimeout(function () {
				$('#insert-msg-before').removeClass('active');
			}, timeout - currIncr);

		}

		function insertMessage(messageEl) {
			$('#insert-msg-before').before(messageEl);
			$('#messages-container-overflow')
				.scrollTop(10000000);
		}

		//Plain text responses
		function textResponse(responseText) {
			var cssClass = "bot__message response";
			var element = $('<div/>').addClass(cssClass).html(responseText);
			insertMessage(element);
		}

		//Multiple choice button responses
		function quickReplyResponse(title, replies) {
			var elCssClass = 'bot__message response quick-reply-message active-question';
			var element = $('<div/>').addClass(elCssClass);
			var elInnerHtml = [];
			_.each(replies, function (reply) {
				var btnCssClass = 'js-quick-reply quick-reply button';
				var btnAttrs = { 'type' : 'button', 'value' : reply };
				var btnElement = $('<input/>').addClass(btnCssClass);
				btnElement.attr(btnAttrs);
				elInnerHtml.push(btnElement);
			});
			
			element.html(elInnerHtml);

			// remember the current active quick reply question so we can remove it's active class later
			activeQuickReplyQuestion = element;
			// render the question in a seperate block
			if (title) {
				textResponse(title);
			}
			insertMessage(element);
		}

		function customPayload(payload) {
			if (payload.fields.livechat.boolValue) {
				//TODO some webtrends stuff as well maybe
				//TODO we could check if the counselors are live (using "bLHNOnline" see ReachOut.js) and do something different if they arent
				
				//Set the global livechat variables so the livechat operator will get the transcript data
				lhnCustom1 = usernameLivechat + '  |  ' + orgLiveChat;

				// Get rid of counselor responses at the end of the transcript since they will be about transferring to a counselor
				// Saves space in the 320 characters we get to transmit to livehelpnow, since we send the end of the conversation
				while (stringTranscript.lastIndexOf('Hope:') > stringTranscript.lastIndexOf('You:')) {
					stringTranscript = stringTranscript.substr(0, stringTranscript.lastIndexOf('Hope:'));
				}
				
				//Restrict to the last 320 characters and switch double quotes to single to avoid string interpolation issues
				lhnCustom3 = stringTranscript.substr(-320).replace(/"/gi, "'");

				$('#aLHNBTN').click();
			}
		}

		//Dialogflow sends us the messages for all types (default, facebook, etc) when sending a response
		//We need this function to tell us if any of the messages are "fancy" in which case, we can ignore default messages
		//Helps avoid double rendering of the same message.
		function hasFancyFormat(messages) {
			var hasThem = false;
			_.each(messages, function (msg) {
				if (msg.platform === 'FACEBOOK') {
					hasThem = true;
				}
			});
			return hasThem;
		}

		function dialogflowApiRequest(data) {
			$.ajax({
				type: 'POST',
				url: '/Dialogflow/intents',
				contentType: 'application/json; charset=utf-8',
				dataType: 'json',
				headers : {
					
				},
				data: data,
				success: function (response) {
					console.log(response);
					//Store the latest context
					currentContext = response.queryResult.outputContexts;
					setTimeout(function () {
						renderBotResponse(response);
					}, 700);
				},
				error: function (err) {
					console.log('dialogflowApiRequest response Error:');
					console.log(err);
				}
			});
		}

		function makeTextRequest(text) {
			//Add the user's message to the chat window
			var element = $('<div class="bot__message user">' + text + '</div>');

			stringTranscript += 'You: ' + text + '<br/>';
			insertMessage(element);

			if (activeQuickReplyQuestion) {
				activeQuickReplyQuestion.removeClass('active-question');
			}

			//Serialize the text from the input and send it to dialogflow for NLU processing
			//Pass along the current context to maintain conversation flow
			var json = JSON.stringify({
				contexts: currentContext,
				queries: [text],
				lang: 'en-US',
				projectId: PROJECT_ID,
				sessionId: SESSION_ID
			});
			dialogflowApiRequest(json);
		}

		$('#bot__form').submit(function (event) {
			event.preventDefault();

			var inputField = $(this).children("#user-message"),
				currentText = inputField.val();

			if (currentText.length > 255) {
				textResponse('Please keep responses to 255 characters or less.  Your message is ' + (currentText.length - 255) + ' too many.');
				return;
			}
			// make sure we don't submit an empty message
			if (currentText !== '') {
				makeTextRequest(currentText);
				//Clear the input now that this message has been added to the chat window
				inputField.val('');
			}
		});

		$(document.body).on('click', '.js-quick-reply', function (event) {
			event.preventDefault();
			if ($(this).val()) {
				makeTextRequest($(this).val());
			}
		});

		$('#send-transcript').click(function (event) {
			event.preventDefault();
			$.ajax({
				type: 'POST',
				url: '/api/ASAMemberService/SendChatEmail',
				contentType: 'application/json; charset=utf-8',
				dataType: 'json',
				data: JSON.stringify(stringTranscript),
				success: function (response) {
					console.log(response);
					textResponse('A copy of our chat has been emailed to you.');
				},
				error: function (response) {
					console.log(response);
				}
			});
			SALT.publish('Standard:Action:Generic', { 'activity_name': 'Chatbot-Transcript-Click', 'activity_transaction': '1'});
		});

		//Make request to welcome event when the script loads...
		//so that the welcome message will display without the user having to type a question
		dialogflowApiRequest(JSON.stringify({
			eventName: 'WELCOME',
			lang : 'en-US',
			projectId: PROJECT_ID,
			sessionId : SESSION_ID
		}));
	});
});
