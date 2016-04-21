/*
 * Simple single-linked list
 */

struct SList {
	void *data;
	struct SList *next;
};
typedef struct SList SList;

static inline SList *
s_list_prepend (SList *list, void *value)
{
	SList *first = (SList *) malloc (sizeof (SList));
	first->next = list;
	first->data = value;
	return first;
}

static inline void
s_list_free (SList *list)
{
	while (list) {
		SList *next = list->next;
		free (list);
		list = next;
	}
}
