'use client';

import { Box, Button, LoadingOverlay, Pagination, TextInput } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { keepPreviousData, useQuery } from '@tanstack/react-query';
import React, { useCallback, useState } from 'react';
import { Api } from '@/lib/api';
import { BookTable } from '@/app/(main)/book/book-table';

export default function Page() {
  const pageSize = 10;
  const [page, setPage] = useState(1);
  const [text, setText] = useState<string | undefined>();
  const [keyword] = useDebouncedValue(text, 500);
  const { data, isPending, error, refetch } = useQuery({
    queryKey: ['book', pageSize, page, keyword],
    queryFn: () =>
      Api.Books.Book.getList({
        page,
        pageSize,
        keyword,
      }),
    placeholderData: keepPreviousData,
  });

  const onSearch = useCallback(
    (value: string) => {
      setText(value);
      setPage(1);
    },
    [keyword, pageSize, page]
  );

  return (
    <>
      <div className="flex flex-col gap-4">
        <div className="flex gap-4 items-center">
          <TextInput
            placeholder="键入书名以搜索..."
            value={text}
            onChange={(e) => onSearch(e.currentTarget.value)}
          />
          <Button onClick={() => refetch()}>刷新</Button>
        </div>
        <Box pos="relative">
          <LoadingOverlay visible={isPending} loaderProps={{ children: 'Loading...' }} />
          {data && (
            <>
              <BookTable data={data.data} />
              <Pagination total={data.totalPage} value={page} onChange={setPage} withEdges />
              {error && <div>Error: {error.message}</div>}
            </>
          )}
        </Box>
      </div>
    </>
  );
}
