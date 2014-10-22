/*
* Copyright (c) 2014  Microsoft Corporation. All Rights Reserved.
*/

#ifndef __COMPATSHIM_H__
#define __COMPATSHIM_H__

/* This is normally gotten from Winsock. */
struct timeval {
	long    tv_sec;         /* seconds */
	long    tv_usec;        /* and microseconds */
};

char *getenv(const char*);

int _getch(void);

#endif /* __COMPATSHIM_H__ */