using System.Runtime.CompilerServices;

// The studio-look Inspectors live in their own assembly (they need Veneer, which is optional) but reuse the
// kit-free helpers of this one: the CanvasGroup fix and the "Collect From Children" action.
[assembly: InternalsVisibleTo("UniMVC.Editor.Studio")]
