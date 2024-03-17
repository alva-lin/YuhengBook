'use client';

import { Button } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import React from 'react';
import { Api } from '@/lib/api';
import { BookTable } from '@/app/(main)/book/book-table';

export default function Page() {
  const pageSize = 10;
  const page = 1;
  const { data, isPending, error, refetch } = useQuery({
    queryKey: ['book', pageSize, page],
    queryFn: () =>
      Api.Books.Book.getList({
        page,
        pageSize,
      }),
  });

  return (
    <>
      Book Management Page
      <br />
      <Button onClick={() => refetch()}>Refetch</Button>
      <br />
      {isPending && <div>Loading...</div>}
      {error && <div>Error: {error.message}</div>}
      {data && <BookTable data={data.data} />}
    </>
  );
}
