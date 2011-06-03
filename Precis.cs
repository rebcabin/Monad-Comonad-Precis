#region Compiler Prologue
using System; using NUnit.Framework; namespace YetAnotherMonadComonad { [TestFixture] public class Precis {
#endregion

    /** /
 __   __    _       _                _   _
 \ \ / /__ | |_    / \   _ __   ___ | |_| |__   ___  _ __
  \ V / _ \| __|  / _ \ | '_ \ / _ \| __| '_ \ / _ \| '__|
   | |  __/| |_  / ___ \| | | | (_) | |_| | | |  __/| |
   |_|\___| \__|/_/   \_\_| |_|\___/ \__|_| |_|\___||_|
  __  __                       _                   _
 |  \/  | ___  _ __   __ _  __| |   __ _ _ __   __| |
 | |\/| |/ _ \| '_ \ / _` |/ _` |  / _` | '_ \ / _` |
 | |  | | (_) | | | | (_| | (_| | | (_| | | | | (_| |
 |_|__|_|\___/|_| |_|\__,_|\__,_|  \__,_|_| |_|\__,_|
  / ___|___  _ __ ___   ___  _ __   __ _  __| |
 | |   / _ \| '_ ` _ \ / _ \| '_ \ / _` |/ _` |
 | |__| (_) | | | | | | (_) | | | | (_| | (_| |
  \____\___/|_| |_| |_|\___/|_| |_|\__,_|\__,_|
 |  _ \ _ __  ___   ___(_) ___
 | |_) | '__|/ _ \ / __| |/ __|
 |  __/| |  |  __/| (__| |\__ \
 |_|   |_|   \___| \___|_||___/


Yet Another Monad and Comonad Precis

A piece of literate C# inspired by Jordan E. Terrell's Maybe monad and
informed by the wikipedia article on "Monads in Functional
Programming,"

http://en.wikipedia.org/wiki/Monad_(functional_programming)

Copyright (c) 2011 Brian Beckman

This work is licensed under the Creative Commons
"Attribution-ShareAlike, CC BY-SA" License. To view a copy of this
license, visit http://creativecommons.org/licenses/by-sa/3.0/; or,
send a letter to Creative Commons, 444 Castro Street, Suite 900,
Mountain View, California 94140, USA.

Version of 2 June 2011

  _   _       _         _   _
 | \ | | ___ | |_  __ _| |_(_) ___  _ __
 |  \| |/ _ \| __|/ _` | __| |/ _ \| '_ \
 | |\  | (_) | |_| (_| | |_| | (_) | | | |
 |_| \_|\___/ \__|\__,_|\__|_|\___/|_| |_|

Haskell notation for types:

    t                the type of some quantity or value

    M t              the type of some constructed value, for
                         instance, the type of monads containing
                         values of type t

    M (M t)          the type of constructed value of constructed
                         values, recursively; parentheses are
                         necessary because the type-constructor
                         notation resembles function application, and
                         that associates to the left, that is, M M t
                         means a type constructor M M applied to a 
                         type t

    t -> u           the type of a function from t to u, that is of a
                         function that consumes values of type t and
                         produces values of type u


Hungarian notation for values or instances:

    t                a value of type t (overloaded on the type
                         notation)

    mt               a value of type M t

    mmt              a value of type M (M t), etc.

    t2u              'a t to a u:' a function of type t -> u

    u4t              'a u for a t:' another function of type t -> u;
                         we may write the same particular function
                         sometimes at t2u and sometimes as u4t,
                         depending on the contextual need


Other Haskellisms as appropriate:

    f x              function application; written f(x) in most
                         programming languages and textbooks; very 
                         high precedence (binds tighter than 
                         anything else)

    f x y            function f applied to two arguments, x and y; is
                         the same as ((f x) y), that is, the function
                         (f x), which is f partially applied to its
                         first argument x, yielding a new function of
                         one remaining argument, applied to the
                         remaining argument y.  In other words, all
                         functions are curried and function-
                         application notation is left-associative

    x `bind` y       means the same as bind x y: the backticks
                         surrounding the use of bind convert it into
                         an infix-style operator

    f . g            'f compose g' is a new function that has the
                         effect (f (g x)) on an argument x; in other
                         words, (f . g) x = (f (g x)); composition
                         binds more weakly than application, meaning
                         that f x . g is (f x) . g and f . g x is 
                         f . (g x).

    f $ x            f applied to the entire right-hand side x; this
                         notation occasionally reduces the number of
                         parentheses since it defeats the ordinary
                         strong left associativity of function
                         application

    \x -> E          lambda notation: the function of x that produces
                         the value of some expression E that may
                         depend on x and on other variables, whose
                         values come from the lexical environment
                         (i.e., the entire notation represents a
                         'closure')

  __  __                       _
 |  \/  | ___  _ __   __ _  __| | ___
 | |\/| |/ _ \| '_ \ / _` |/ _` |/ __|
 | |  | | (_) | | | | (_| | (_| |\__ \
 |_|  |_|\___/|_| |_|\__,_|\__,_||___/

================================================================
D e f i n i t i o n
================================================================

The standard definition presents 'return' and 'bind,' of the following
signatures:

    return :: t -> M t
    
taking a value of type t and producing mt -- a monad containing values
of type t (a monad of t's), and

    bind :: M t -> (t -> M u) -> M u    
    
taking mt -- a monad of t's, and t2mu -- a function that takes t's to
u's, and producing a monad of u's.  The intuition is 'concatenate' and
then 'flatten one level."

================================================================
L a w s  
================================================================

The monad laws, which govern any implementation of bind and return,
are the following:

Law 1:

    mt `bind` return = mt

A monad of t's bound to return must produce the original monad.
Another, more terse way of writing this is

    (flip bind) return = id

The function (flip bind) -- a copy of bind that just takes its
arguments in opposite order, partially applied to the function return,
produces id, the unique function of type t -> t that simply produces
its input.

Law 2:

    (return t) `bind` t2mu = t2mu t

A monad mt produced by applying return to some t and then bound to
some function t2mu of type (t -> M u) must produce the same value as
t2mu applied to t in the first place.

These two laws express the notion that return must be a kind of
neutral lifting function that puts a value t into mt -- a monad of
t's, without doing anything else to it.

Law 3:

    (mt `bind` t2mu) `bind` u2mv =

    mt `bind` (\t -> (t2mu t) `bind` u2mv)

The associative law, reading 
* a monad of t's, mt,
* bound to t2mu, a function of type t -> mu
* producing mu -- a monad of u's, by the definition of bind
* and then bound to a function of type u2mv
* producing mv -- a monad of v's, again by the definition of bind
is the same as 
* mt bound to the function of t that applies t2mu to t
* producing a monad of u's
* and then binding the result to u2mv.  

This expression of the law looks a little grimy, but that's only
because of the need to insert the intermediate function \t -> ... in
the right-associative form on the right-hand side.  If you squint a
bit, the law really just looks like

    (a op b) op c = a op (b op c)

which is the associative law in its most familiar guise.

That's it.  Any pair of functions that satisfies the above definitions
and laws constitutes a monad.  They don't have to be named return and
bind, of course, they just must satisfy the definitions and laws.

See the wikipedia article for many examples.

                ************************************************
                In general, it is good programming practice to
                write unit tests for these laws.
                ************************************************

================================================================
A l t e r n a t i v e   F o r m u l a t i o n:
================================================================

An alternative and equivalent definition presents 'return,' 'join,'
and 'fmap.'

Join just performs one level of flattening.  It takes mmt -- a monad
of monads of t's, and produces mt -- a monad of t's.  Its type is the
following:

    join :: M (M t) -> M t

Fmap Takes t2u -- a transform of type (t -> u), and produces mt2mu --
a function of type (M t -> M u) that "does the same thing" to the
values in the monad:

    fmap :: (t -> u) -> (M t -> M u)

The relation between the bind-return formulation and the join-fmap-
return formulation is as follows:

Let t2u be a transform of type (t -> u).  Let (fmap t2u) be mt2mu -- a
function of type (M t -> M u), that transforms mt -- a monad of t's,
and produces mu -- a monad of u's.

Equivalance 1:

    (fmap t2u) mt === mt `bind` (\t -> return (t2u t))
    
Bind takes mt -- a monad of t's, and some t2mu -- a function that
takes t's to mu's -- monads of u's, and produces another mu, a single
monad of u's -- it lifts and flattens one level.  That's just what
((fmap t2u) mt) does.

Equivalence 2:

    join mmt === mmt `bind` id

First, examine the right-hand side of the equivalence.  The first
argument to 'bind' must be a monad of values of some type.  Let 'mmt'
be a monad of mt's -- a monad of nested monads.  The second argument
to 'bind' must be a function that takes values from the input monad
and produces monads of values of potentially some other type.  'id,'
in this case, takes values from the input monad mmt, that is, values
mt of type M t, and produces monads of values of potentially some
other type; in this case, monads of value of type mt -- monads of
monads of t's.  The types of the two expressions 'join mmt' and 'mmt
`bind` id' evidently match, and Equivalence 2 requires that the
results match in any application of these functions.

Equivalence 3:

    mt `bind` t2mu === join ((fmap t2mu) mt)
    
Let t2mu have type (t -> M u).  Let (fmap t2mu) produce mt2mmu -- a
function taking mt's -- monads of t's, to mmu's -- monads of monads of
u's.  Applying mt2mmu to mt -- a monad of t's, produces mmu -- a
(nested) monad of mu's, and applying join to that result flattens out
one level of monad, producing an mu -- a monad of u's.

Both sides of the equivalence have the same type, and the equivalence
requires that they always have the same value.

The three equivalences establish the equivalence of the bind-return
formulation of monads and the join-fmap-return formulation of monads.

The following laws also hold:

-------- L a w s --------

Law 4:

    fmap id = id

Id works on values of any type.  Lifting id to operate on monads must
preserve its semantics.
    
Law 5:

    fmap (u2w . t2u) = (fmap u2w) . (fmap t2u)
    
Let (u2w . t2u) be t2w, the composition of the functions u2w and t2u
having type (t -> w).  Let fmap applied to t2w be mt2mw, of type M t
-> M w.

To the right-hand side: let (fmap t2u) be mt2mu and (fmap u2w) be
mu2mw, and the composition of these two functions be mt2mw, of type M
t -> M w.  The law requires that the results be the same in any
application of these functions.
    
Law 6:

    return . t2u = (fmap t2u) . return

(return . t2u) is the composition of return and t2u.  That means that
(return . t2u) applies t2u first and then applies return to the
result, producing a monad, mu, of type M u.

Now, examine the right-hand side.  Let return be applied to a value of
type t, producing mt -- a monad of t's.  Let (fmap t2u) be mt2mu -- a
function from monads of t's to monads of u's.  The law requires that
the left-hand side and the right-hand side produce the same values in
any application.

Law 7:

    join . (fmap join) = join . join
    
Join takes monads of monads to monads; rewrite join as mmt2mt.  Thus,
(fmap mmt2mt) is an mmmt2mmt: it prduces a value mmt of type (M (M t))
when applied to a value mmmt of type (M (M (M t))).  Applying join to
mmt, the result of applying mmmt2mmt to some mmmt, produce mt, so
(join . (fmap join)) must take an mmmt and produce an mt.  It's clear
that (join . join), given an mmmt will produce an mt.  The law
requires that both sides produce the same values when applied to any
mmmt.

Law 8:

    join . (fmap return) = join . return = id

The left-hand side is the composition of join and (fmap return).
Rewrite return as t2mt, and (fmap return) as mt2mmt.  Applying mt2mmt
to some mt, a monad of t's, produces mmt -- a monad of monad of t's,
and applying join to that mmt produces mt.  

Now, to the right-hand side.  Applying return to mt -- a monad of t's,
produces mmt -- a monad of monads of t's.  Applying join to that mmt
produces mt, a monad of t's.  The law requires that both expressions
have id semantics.

Law 9:
    
    join . (fmap (fmap t2u)) = (fmap t2u) . join
    
Let (fmap t2u) be mt2mu.  Let (fmap mt2mu) be mmt2mmu.  Apply it to
some mmt, producing mmu, then apply join, producing mu.  

On the right-hand side, apply join to some mmt producing an mt, then
apply mt2mu producing an mu.  The law requires both sides to produce
the same results.

                ************************************************
                In general, it is good programming practice to
                write unit tests for these laws.
                ************************************************
   ____                                      _
  / ___|___  _ __ ___   ___  _ __   __ _  __| | ___
 | |   / _ \| '_ ` _ \ / _ \| '_ \ / _` |/ _` |/ __|
 | |__| (_) | | | | | | (_) | | | | (_| | (_| |\__ \
  \____\___/|_| |_| |_|\___/|_| |_|\__,_|\__,_||___/


================================================================
D e f i n i t i o n
================================================================

    Type constructor W t

                     conventionally written W to suggest an 'upside-
                     down M,' that is, a flip-side of a monad
                         
    extract :: W t -> t

                     extract is the flip-side of return.

    extend :: (W t -> u) -> W t -> W u

                     extend is the flip-side of bind. Flip the M
                     upside-down into a W, flip the arrows, and flip
                     the order of arguments to extend (bind takes the
                     monad of t's, mt, first, then the t2mu lifter
                     function; extend takes the extractor function,
                     wt2u, first, then the target comonad of t's, wt).

                     Extend, partially applied to some wt2u, is a
                     function from some wt to a wu, that is, a wt2wu
                     of type (W t -> W u).  To understand most of the
                     expressions below, keep this aspect of extend in
                     mind.
                     
================================================================
L a w s
================================================================

Law 1':

    extend extract = id

This is the flip-side of Law 1, which is (flip bind) return = id.

extract takes a wt -- a comonad of t's, and produces a t.  extend
takes a wt2u and produces a wu.  extract is a wt2u -- a legitimate
argument for the function parameter of extend -- extract is a wt2u in
which t = u, that is, extract is a wt2t.  The law requires that extend
applied to extract have id semantics.

Law 2':

    extract . (extend wt2u) = wt2u

Let extend, applied to some wt2u, be wt2wu -- a function from some wt
to some wu, that is, a function of type (W t -> W u).  Imagine
applying this wt2wu to some wt, producing a wu, and then applying
extract to that wu, producing a u.  That's the meaning of the
left-hand side.  The law requires that the results of that composition
be the same as the result of applying wt2u in the first place.  In
other words, that

    (extract . (extend wt2u)) wt = wt2u wt

As usual, we may remove the argument wt from the extreme right-hand
sides of both sides of the equation, producing the original expression
of law 2'.

Law 3':

    (extend wu2v) . (extend wt2u) =
    extend (wu2v . (extend wt2u))

First, the left-hand side.  Let extend wt2u be wt2wu.  Apply this
wt2wu to some wt, producing a wu.  Apply (extend wu2v), which is a
wu2wv, to that wu, producing a wv.  Thus, the result of the
composition (extend wu2v) . (extend wt2u) must be a wt2wv.  

Now, the right-hand side.  Composing wu2v with (extend wt2u), which is
a wt2wu, produces a wt2v.  Extending that wt2v produces a wt2wv.


================================================================
A l t e r n a t i v e   F o r m u l a t i o n:
================================================================

Make duals by flipping arrows and inverting M's to W's.

fmap is self-dual.

We already have the dual of return: extract.

The dual of join is 'duplicate;' its type is as follows:

    duplicate :: W t -> W (W t)

-------- L a w s --------

Law 4':

    extract . duplicate = id

To unravel an equation like this, insert the same argument at the
extreme right of both sides of the equation. duplicate takes wt, a
comonad of t's, so the type of this argument must be W t.  Rewrite the
equation as

    (extract . duplicate) wt = id wt

then substitute the expansion of the composition extract . duplicate:

    (extract . duplicate) wt = extract (duplicate wt)

Let duplicate wt be wwt -- a comonad of comonads of t's.  extract
removes one level of comonadic structure.  Extract wwt has type W t,
and the law requires that any such application of extract . duplicate
must re produce its input exactly as does id.

To recover the terse, argument-free statement of the law, remove wt
from the extreme right of each side of the equation.

Law 5':

    (fmap extract) . duplicate = id

First, examine the left-hand side.  Supply an argument wt -- a comonad
of t's, to both sides of the equation:

    ((fmap extract) . duplicate) wt = id wt

being careful to add the extra parentheses because function
application binds tighter than function composition. Expand the
function composition:

    ((fmap extract) . duplicate) wt = (fmap extract) (duplicate wt)

Let the value of (duplicate wt) be wwt -- a comonad of comonads of
t's, of type (W (W t)).

fmap takes any function and promotes it into the comonad, so takes any
t2u to a wt2wu.  extract takes a wt and produces a t, therefore,
extract is a wt2t.  (fmap extract) is an (fmap wt2t), thus a wwt2wt.
Applied to the the wwt produced by (duplicate wt), it produces a wt,
and the law requires that this be the same wt as input in the first
place.  The expression of the law type-checks.

Law 6':

    duplicate . duplicate = (fmap duplicate) . duplicate

First, consider the left-hand side. Supplying arguments to both sides
of the equation and expanding the composition on the left-hand side:
((duplicate . duplicate) wt) is (duplicate (duplicate wt)) for any wt
-- a comonad of t's.  Let (duplicate wt) be wwt and (duplicate
(duplicate wt)) be wwwt.

Now consider the right-hand side. duplicate is a wt2wwt and (fmap
duplicate) is a wwt2wwwt.  Composed to duplicate, it must produce a
wwwt, and the law requires that the two wwwt's be the same.

The extend-extract formulation is related to the duplicate-fmap-
extract formulation as follows:

Equivalence 1':

    fmap t2u = extend (t2u . extract)

The left-hand side produces a wt2wu -- a function of type 
(W t -> W u).  

The equation states that the right-hand side must equal this wt2wu,
but as the result of extend applied to some function.

The function argument of extend, namely (t2u . extract) must be a wx2y
-- of type W x -> y for some x and y, and the result of the partial
application of extend to its function argument must thus be a wx2wy.
Solving for x and y in this case, where wt2wu from the left equals
this wx2wy, reveals that x = t and y = u, meaning that (t2u . extract)
must be a wt2u -- of type W t -> u.

The equivalence type-checks.

A slightly more modern style favors removing arguments from equations
as much as possible, yielding for Equivalence 1' the following:

    fmap = \t2u -> extend (t2u . extract)

Equivalence 2':

    duplicate = extend id

The left-hand side, by definition, is a wt2wwt, taking a wt -- of type
W t -- and producing a wwt -- of type W (W t).

On the right-hand side, extend's first argument must be a wx2y of type
W x -> y. id satisfies that requirement if y = W x, meaning that id is
a wx2wx in this context.  extend applied to this wx2wx produces a
wx2wwx -- of type W x -> W (W x).  Solving for x yields x = t, so the
equivalence type-matches a wt2wwt on each side.

Equivalence 3':

    extend wt2u = (fmap wt2u) . duplicate

First, Let the left-hand side, (extend wt2u), be wt2wu. Given a wt, it
produces a wu.

Now supply an argument, wt, on the right-hand side and expand the
composition, proceeding to calculate as before:

    ((fmap wt2u) . duplicate) wt = 
    (fmap wt2u) (duplicate wt) =
    (fmap wt2u) wwt =
    wwt2wu wwt =
    wu

The equation type-matches.

/**/
#region Compiler Epilogue
} }
#endregion