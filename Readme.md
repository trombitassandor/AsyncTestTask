# Description of the problem:

Please write code in either typescript or C# that solves the problem below.

Problem Description:

You are given a stack, and will be pushing numbers from 0 to N in sequence into this stack. At

any time there may be any numbers popped from the stack and printed to screen.

# Sample

Please validate if a given sequence of numbers could be the result of the above procedure.

Sample 1 input:

0 1 2 3

Sample 1 output:

True

Explanation:

push 0

pop 0

push 1

pop 1

push 2

pop 2

push 3

pop 3

Sample 2 input:

0 3 1 2

Sample 2 output:

False

Explanation:

push 0

pop 0

push 1

push 2

push 3

pop 3


We can't pop 1 now because at this point the stack is:

[1 2 (top)]

So this is invalid.

# The approach to the problem:

ProgramObsolete is the first, more functional programming implementation.

Program is a cleaned up more oop version of the implementation.

SampleValidationMethod validates the samples controlling the NumberPushMethod and NumberPopMethod.

NumberPushMethod pushes numbers from 0 to N into the stack.

NumberPopMethod pops numbers of Input from the stack.

QueuedLock was needed to ensure the access order in addition to locking for the shared stack pop push methods.

The solution was implemented in a thread safe async way as the operations could happen at any time.

The random access is imitated with task delays, async functioning and separate threading.

Test assignment from SeriesAI
