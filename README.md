PhoneticSearch
==============

Modified version of the soundex algorithm (http://en.wikipedia.org/wiki/Soundex)

Algorithm is as follows:

1.  All non-alphabetic characters are ignored

2.  Word case is not significant

3.  After the first letter, any of the following letters are discarded: A, E, I, H, O, U, W, Y.

4.  The following sets of letters are considered equivalent
    > A, E, I, O, U
    > C, G, J, K, Q, S, X, Y, Z
    > B, F, P, V, W
    > D, T
    > M, N
    > All others have no equivalent

5.  Any consecutive occurrences of equivalent letters (after discarding letters in step 3) are considered as 
a single occurrence
