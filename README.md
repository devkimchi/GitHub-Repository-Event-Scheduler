# GitHub Repository Event Scheduler #

This is the code sample for event scheduler to GitHub, using Durable Functions.


## Getting Started ##

### Deploy Function App ###

Deploy the Durable Function app to Azure. Alternatively, you can run this app on your local machine with [Azure Storage Emulator](https://aka.ms/dk/storage-emulator).


### Set up App Settings ###

Once the app is deployed to Azure, add the configuration values below to App Settings. Alternatively, update `local.settings.json` to run the app on your local machine.

* `Duration__Max`: The maximum duration is seven days in the format of `d.HH:mm:ss`, which is `7.00:00:00`. This is due to the limitation of Durable Functions.
* `GitHub__AuthKey`: This is the basic authentication key. To get the basic authentication key, get a personal access token with your username and build the key.
* `GitHub__BaseUri`: This must be `https://api.github.com/`.
* `GitHub__Endpoints__Dispatches`: This must be `dispatches`
* `GitHub__Headers__Accept`: This must be `application/vnd.github.v3+json`
* `GitHub__Headers__UserAgent`: This can be an arbitrary value.


### Set Schedule ###

In order to set schedule, send an HTTP request to the following URL with the payload:

```bash
curl -X POST https://<functions_app_name>.azurewebsites.net/api/orchestrators/schedule-event?code=<function_auth_key> \
     -H "Content-Type: application/json" \
     -d '{ "owner": "<owner_or_organisation>", "repository": "<repository>", "issueId": <issue_id>, "schedule": "<yyyy-MM-ddTHH:mm:ss+hh:mm>" }'
```

An example of the payload might look like:

```json
{
  "owner": "devkimchi",
  "repository": "GitHub-Repository-Event-Scheduler",
  "issueId": 1,
  "schedule": "2020-03-22T07:30:00+09:00"
}
```

It schedules to send a `repository_dispatch` event to GitHub at 7.30am (Korean Standard Time; KST) on March 22nd, 2020.


### Get List of Schedules ###

In order to get the list of schedules, send an HTTP request to the following URL:

```bash
curl -X GET https://<functions_app_name>.azurewebsites.net/api/orchestrators/schedule-event/schedules?code=<function_auth_key> \
     -H "Content-Type: application/json"
```

It returns all the list of schedules in the descending order of scheduled date/time.


### Get the Schedule Details ###

In order to get the schedule details, send an HTTP request to the following URL:

```bash
curl -X GET https://<functions_app_name>.azurewebsites.net/api/orchestrators/schedule-event/schedules/<instance_id>?code=<function_auth_key> \
     -H "Content-Type: application/json"
```

It returns the schedule details.


## Contribution ##

Your contributions are always welcome! All your work should be done in your forked repository. Once you finish your work with corresponding tests, please send us a pull request onto our `dev` branch for review.


## License ##

This is released under [MIT License](http://opensource.org/licenses/MIT)

> The MIT License (MIT)
>
> Copyright (c) 2019 [Dev Kimchi](https://devkimchi.com)
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
