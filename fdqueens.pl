/*
    Purpose	: answer https://stackoverflow.com/q/65945554/874024
			: original FD Queens sample by Markus Triska
    Usage	: foreign predicate queen_paint(Row, Col, Status) is called each
			: time a FD var status change, by means of a frozen goal
*/

:- module(fdqueens, [fdqueens/2]).
:- use_module(library(clpfd)).

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   Constraint posting.
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

n_queens(N, Qs) :-
        length(Qs, N),
        Qs ins 1..N,
        safe_queens(Qs).

safe_queens([]).
safe_queens([Q|Qs]) :- safe_queens(Qs, Q, 1), safe_queens(Qs).

safe_queens([], _, _).
safe_queens([Q|Qs], Q0, D0) :-
        Q0 #\= Q,
        abs(Q0 - Q) #\= D0,
        D1 #= D0 + 1,
        safe_queens(Qs, Q0, D1).

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   Animation.

   For each N of the domain of queen Q, a reified constraint of the form

      Q #= N #<==> B

   is posted. When N vanishes from the domain, B becomes 0. A frozen
   goal then emits (was - PostScript) instructions for graying out the field.
   When B becomes 1, the frozen goal emits instructions for placing
   the queen. On backtracking, the field is cleared.
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

animate(Dia, Qs) :- animate(Dia, Qs, Qs, 1).

animate(_, [], _, _).
animate(Dia, [_|Rest], Qs, N) :-
        animate_(Dia, Qs, 1, N),
        N1 #= N + 1,
        animate(Dia, Rest, Qs, N1).

animate_(_, [], _, _).
animate_(Dia, [Q|Qs], C, N) :-
        freeze(B, queen_value_truth(Dia, C,N,B)),
        Q #= N #<==> B,
        C1 #= C + 1,
        animate_(Dia, Qs, C1, N).

% queen_paint could actually be queen_value_truth, moving that logic inside

queen_value_truth(_Dia, Q, N, 1) :- writeln(queen_paint(Q, N, place)), queen_paint(Q, N, place).
queen_value_truth(_Dia, Q, N, 0) :- writeln(queen_paint(Q, N, gray)),  queen_paint(Q, N, gray).
queen_value_truth(_Dia, Q, N, _) :- writeln(queen_paint(Q, N, clear)), queen_paint(Q, N, clear), fail.

fdqueens(Dia, N) :-
        N #> 0,
        n_queens(N, Qs),
        animate(Dia, Qs),
        label(Qs),
        writeln('Solution'(Qs)).
