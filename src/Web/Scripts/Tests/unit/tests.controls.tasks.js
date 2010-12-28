﻿$(function () {
    module("tasks control tests",
        {
            setup: function () {
                $("#hidden_test_dom").append($('<div id="tasks"/>'));
            },
            teardown: function () {
                $("#hidden_test_dom").empty();
            }
        }
        );

    test("smoke test", function () {

        var control = new tasksControl();
        ok(control != null);
    });

    test("add new task to control", function () {

        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null };

        // act
        control.addTask(task);

        // post
        var currentTasks = control.tasksCount();
        ok(currentTasks == 1, "task has not been added to control");
    });

    test("add new task add tasks ui", function () {

        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null };

        // act
        control.addTask(task);

        // post
        var currentTasks = $('#tasks .task').size();
        ok(currentTasks == 1, "task has not been added to ui");
    });

    test("remove task from control", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null };
        control.addTask(task);

        // act
        control.removeTask(task.id);

        // assert
        var currentTasks = control.tasksCount();
        ok(currentTasks == 0, "task has not been removed from control");
    });

    test("remove task from ui", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null };
        control.addTask(task);

        // act
        control.removeTask(task.id);

        // assert
        var currentTasks = $('#tasks .task').size();
        ok(currentTasks == 0, "task has not been removed from ui");
    });

    test("task has description field", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var description = task.children('.description');
        ok(description.length == 1, "description span is absent in task");
    });

    test("task has start button", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var start = task.children('.start').children('a');
        ok(start.length == 1, "start is absent in task");
    });

    test("task start button href is initialized", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var start = task.children('.start').children('a');
        ok(start.length == 1, "start is absent in task");
        var href = start.attr('href');
        same(href, "/tasks/start/0", "task href is incorrect");
    });

    test("task has stop button", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var start = task.children('.stop');
        ok(start.length == 1, "stop is absent in task");
    });

    test("task stop button href is initialized", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var stop = task.children('.stop').children('a');
        ok(stop.length == 1, "start is absent in task");
        var href = stop.attr('href');
        same(href, "/tasks/stop/0", "task href is incorrect");
    });

    test("task has delete button", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var start = task.children('.delete');
        ok(start.length == 1, "delete is absent in task");
    });

    test("task delete button href is initialized", function () {
        // assert
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 0 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var stop = task.children('.delete').children('a');
        ok(stop.length == 1, "delete is absent in task");
        var href = stop.attr('href');
        same(href, "/tasks/delete/0", "delete href is incorrect");
    });

    test("task timer is initialized", function () {
        var control = new tasksControl($('#tasks'));
        var task = { id: 0, description: "task 1", status: 0, createdDate: null, startedDate: null, stoppedDate: null, spent: 77 };

        // act
        control.addTask(task);

        // assert
        var task = $('#tasks .task:first-child');
        ok(task != null, "could not get task from div");
        var timer = task.children('.timer').html();
        same(timer, "01:17", "timer has not been initialized");
    });
});