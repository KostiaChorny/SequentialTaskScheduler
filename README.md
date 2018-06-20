# Sequential Task Scheduler

Implement a C# class which responsibility is to sequentially execute tasks. Task is a code to be executed (e.g. a delegate).
Class should fulfill the following requirements:
- Tasks can be added at any time by any number of clients.
- Tasks are executed sequentially in order they were added.
- Only one task can be executed at a time.
- Client code should use only one method that influence on execution flow: for ex. AddForExecution(…)
