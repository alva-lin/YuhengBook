'use client';

import { useCallback, useState } from 'react';

import Link from 'next/link';

import { Text } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';

import MyPagination from '@/components/Pagination';
import { Api } from '@/lib/api';

export default function Page({ params: { bookId } }: { params: { bookId: number } }) {
  const { data: book } = useQuery({
    queryKey: ['book', bookId],
    queryFn: () => Api.Books.Book.get(bookId),
  });

  const [chapters, setChapters] = useState(() => {
    return book?.chapters.slice(0, 30) || [];
  });

  const onPaginationChange = useCallback(
    (page: number, pageSize: number, orderBy: 'asc' | 'desc') => {
      let data = book?.chapters || [];
      if (orderBy === 'asc') {
        data = data.sort((a, b) => a.order - b.order);
      } else {
        data = data.sort((a, b) => b.order - a.order);
      }

      setChapters(data.slice((page - 1) * pageSize, page * pageSize));
    },
    [book]
  );

  return (
    <div className="my-12 flex justify-center items-center">
      <div className="max-w-[1200px] flex flex-col gap-8 items-center">
        {book && (
          <>
            <div className="text-4xl text-center font-bold">{book.name}</div>

            <MyPagination count={book.chapterCount} onChange={onPaginationChange} showOrder />
            <div className="w-full grid grid-cols-1 xs:grid-cols-2 md:grid-cols-3 gap-4 justify-between">
              {chapters.map((item) => (
                <div>
                  <Text component={Link} href={`/book/${bookId}/chapter/${item.order}`}>
                    {item.title}
                  </Text>
                </div>
              ))}
            </div>
            <MyPagination count={book.chapterCount} onChange={onPaginationChange} showOrder />
          </>
        )}
      </div>
    </div>
  );
}
