﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Trackyt.Core.DAL.DataModel;
using Trackyt.Core.DAL.Extensions;
using Trackyt.Core.DAL.Repositories;
using Trackyt.Core.Services;
using Web.API.v11.Model;

namespace Web.API.v11.Controllers
{
    // TODO: implemented quickly by copy-pasting code of v1, it have to be refactored to use common codebase
    // TODO: refactor, refactor, refactor!

    public class ApiV11Controller : Controller
    {
        private IApiService _api;
        private ITasksRepository _tasks;
        private IMappingEngine _mapper;

        public ApiV11Controller(IApiService auth, ITasksRepository repository, IMappingEngine mapper)
        {
            _api = auth;
            _tasks = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public JsonResult Authenticate(string email, string password)
        {
            var success = true;
            var apiToken = _api.GetApiToken(email, password);

            if (apiToken == null)
            {
                success = false;
            }

            return Json(
                new
                {
                    success = success,
                    data = new { apiToken = apiToken }
                });
        }

        // Tasks

        // GET tasks/all

        [HttpGet]
        public JsonResult All(string apiToken)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }


            var tasks = CreateTasksList(userId);

            return Json(
                new
                {
                    success = true,
                    data = new { tasks = tasks }
                },
                JsonRequestBehavior.AllowGet);
        }

        // POST tasks/add
        // TODO: API handle description == null case
        [HttpPost]
        public JsonResult Add(string apiToken, string description)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }

            var task = new Task { Description = description, UserId = userId, Status = (int)TaskStatus.None };
            _tasks.Save(task);

            return Json(
                new
                {
                    success = true,
                    data = new { task = CreateTaskDescriptor(task) }
                });
        }

        // DELETE tasks/delete
        // TODO: API correct to use taskId
        // TODO: API add integration test
        [HttpDelete]
        public JsonResult Delete(string apiToken, int taskId)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }

            var task = _tasks.Tasks.WithId(taskId);
            if (task != null)
            {
                _tasks.Delete(task);
            }

            return Json(
                new
                {
                    success = true,
                    data = new { id = taskId }
                });
        }

        // PUT tasks/start

        [HttpPut]
        public JsonResult Start(string apiToken, int taskId)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }

            var task = _tasks.Tasks.WithId(taskId);

            StartAndSave(task);

            return Json(
                new
                {
                    success = true,
                    data = CreateTaskDescriptor(task) 
                });
        }

        // PUT tasks/start/all

        [HttpPut]
        public JsonResult StartAll(string apiToken)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }

            var allTasks = _tasks.Tasks.WithUserId(userId);
            foreach (var task in allTasks)
            {
                StartAndSave(task);
            }

            return Json(
                new
                {
                    success = true,
                    data = (string)null
                });
        }

        // PUT tasks/stop/all

        [HttpPut]
        public JsonResult StopAll(string apiToken)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }

            var allTasks = _tasks.Tasks.WithUserId(userId);
            foreach (var task in allTasks)
            {
                StopAndSave(task);
            }

            return Json(
                new
                {
                    success = true,
                    data = (string)null
                });
        }

        [HttpPut]
        public JsonResult Stop(string apiToken, int taskId)
        {
            var userId = _api.GetUserIdByApiToken(apiToken);

            if (userId == 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        data = (string)null
                    });
            }


            var task = _tasks.Tasks.WithId(taskId);

            StopAndSave(task);

            return Json(
                new
                {
                    success = true,
                    data = CreateTaskDescriptor(task) 
                });
        }

        private IList<TaskDescriptor> CreateTasksList(int userId)
        {
            return _tasks.Tasks.WithUserId(userId).Select(t => CreateTaskDescriptor(t)).ToList();
        }

        private TaskDescriptor CreateTaskDescriptor(Task t)
        {
            return new TaskDescriptor
            {
                id = t.Id,
                description = t.Description,
                createdDate = t.CreatedDate,
                startedDate = t.StartedDate,
                stoppedDate = t.StoppedDate,
                status = t.Status,
                spent = GetTaskActualWork(t)
            };
        }

        private int GetTaskActualWork(Task t)
        {
            var actualWork = t.ActualWork;

            if (t.Status == (int)TaskStatus.Started)
            {
                actualWork += GetDifferenceInSeconds(t.StartedDate, DateTime.UtcNow);
            }

            return actualWork;
        }

        private int GetDifferenceInSeconds(DateTime? start, DateTime? stop)
        {
            if (start == null)
            {
                return 0;
            }

            if (stop == null)
            {
                return (int)(DateTime.UtcNow - start).Value.TotalSeconds;
            }

            return Convert.ToInt32(Math.Floor((stop - start).Value.TotalSeconds));
        }

        private void StartAndSave(Task task)
        {
            if (task.Status == (int)TaskStatus.None || task.Status == (int)TaskStatus.Stopped)
            {
                task.Status = (int)TaskStatus.Started;
                task.StartedDate = DateTime.UtcNow;
                task.StoppedDate = null;
                _tasks.Save(task);
            }
        }

        private void StopAndSave(Task task)
        {
            if (task.Status == (int)TaskStatus.Started)
            {
                task.Status = (int)TaskStatus.Stopped;
                task.StoppedDate = DateTime.UtcNow;
                task.ActualWork += GetDifferenceInSeconds(task.StartedDate, task.StoppedDate);

                _tasks.Save(task);
            }
        }
    }
}