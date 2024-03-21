export default function Article({ text }: { text: string }) {
  const paragraphs = text.split('\n\n');

  return (
    <div className="article">
      {paragraphs.map((paragraph, index) => (
        <p key={index} className="mb-4 text-xl indent-8">
          {paragraph}
        </p>
      ))}
    </div>
  );
}
